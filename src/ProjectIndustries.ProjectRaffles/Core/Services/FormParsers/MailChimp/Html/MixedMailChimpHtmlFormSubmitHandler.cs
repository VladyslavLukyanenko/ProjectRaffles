using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Html
{
  public class MixedMailChimpHtmlFormSubmitHandler : IFormSubmitHandler
  {
    private const string CaptchaConfirmPath = "/subscribe/confirm-captcha";
    private readonly IMailchimpService _mailchimpService;
    private readonly CookieContainer _cookieContainer;
    private readonly ICaptchaSolveService _captchaSolveService;

    private readonly Uri _submitAsEncodedUri;

    public MixedMailChimpHtmlFormSubmitHandler(IMailchimpService mailchimpService, CookieContainer cookieContainer,
      Uri submitUri, ICaptchaSolveService captchaSolveService)
    {
      _mailchimpService = mailchimpService;
      _cookieContainer = cookieContainer;
      _submitAsEncodedUri = submitUri;
      _captchaSolveService = captchaSolveService;
    }

    public async ValueTask<FormSubmitResult> SubmitAsync(Uri originUrl, IEnumerable<Field> fields,
      CancellationToken ct = default)
    {
      var submitAsJsonUrl = new Uri(_submitAsEncodedUri.ToString().Replace("/post", "/post-json"));
      var httpClient = CreateHttpClient();
      var jsonSubmitResponse = await SubmitAsync(originUrl, fields, httpClient, submitAsJsonUrl, ct);
      if (jsonSubmitResponse.IsCaptchaError)
      {
        var captchaSolvePathUrlBuilder = new UriBuilder(_submitAsEncodedUri)
        {
          Query = string.Join("&", jsonSubmitResponse.Params.Select(p => $"{p.Key}={p.Value}"))
        };

        var pageWithCaptchaRequest = new HttpRequestMessage(HttpMethod.Get, captchaSolvePathUrlBuilder.Uri);
        var pageWithCaptchaResponse = await httpClient.SendAsync(pageWithCaptchaRequest, ct);
        if (!pageWithCaptchaResponse.IsSuccessStatusCode)
        {
          await pageWithCaptchaResponse.FailWithRootCauseAsync(ct: ct);
        }

        var pageWithCaptchaContent = await pageWithCaptchaResponse.Content.ReadPossiblyGZippedAsStringAsync(ct);
        var result = await ConfirmCaptchaAsync(_submitAsEncodedUri, pageWithCaptchaContent, httpClient, ct);
        return result
          ? FormSubmitResult.Successful()
          : FormSubmitResult.Failed(jsonSubmitResponse.Raw);
      }

      return jsonSubmitResponse.IsSuccessful
        ? FormSubmitResult.Successful()
        : FormSubmitResult.Failed(jsonSubmitResponse.Raw);
    }

    private async ValueTask<JsonSubmitResponse> SubmitAsync(Uri originUrl, IEnumerable<Field> fields, HttpClient httpClient,
      Uri submitAsJsonUrl, CancellationToken ct)
    {
      var submitFields = fields.ToList();

      var timestamp = await _mailchimpService.GetUnixTime();
      submitFields.Add(new HiddenField("_", timestamp));
      var origin = originUrl.GetLeftPart(UriPartial.Authority);
      httpClient.DefaultRequestHeaders.Add("origin", origin);
      httpClient.DefaultRequestHeaders.Add("referer", originUrl.ToString());

      var jqueryId = await _mailchimpService.GeneratejQueryId();
      submitFields.Insert(0, new HiddenField("c", jqueryId));

      var queryParams = submitFields.Where(f => !(f is CheckboxField cb) || cb.IsChecked)
        .Select(_ => new KeyValuePair<string, string>(_.SystemName, _.Value?.ToString()))
        .ToList();

      var previousQueryParams = QueryStringUtil.Parse(submitAsJsonUrl.Query);
      foreach (var param in previousQueryParams)
      {
        queryParams.AddRange(param.Select(v => new KeyValuePair<string, string>(param.Key, v)));
      }

      var query = string.Join("&", queryParams.Select(p => $"{p.Key}={p.Value}"));
      var submitUrlBuilder = new UriBuilder(submitAsJsonUrl) {Query = query};
      var requestUri = submitUrlBuilder.Uri;
      var message = new HttpRequestMessage(HttpMethod.Get, requestUri);

      var postAccount = await httpClient.SendAsync(message, ct);
      string content = await postAccount.Content.ReadPossiblyGZippedAsStringAsync(ct);
      var rawJsonSubmitResponse = content.Replace(jqueryId, "")
        .TrimStart('(')
        .TrimEnd(')');
      var jsonSubmitResponse = JsonConvert.DeserializeObject<JsonSubmitResponse>(rawJsonSubmitResponse);
      jsonSubmitResponse.Raw = content;

      return jsonSubmitResponse;
    }

    private HttpClient CreateHttpClient()
    {
      var handler = new HttpClientHandler
      {
        UseCookies = true,
        CookieContainer = _cookieContainer
      };
      var httpClient = new HttpClient(handler);
      httpClient.DefaultRequestHeaders.Add("User-Agent",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
      httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
      httpClient.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");
      httpClient.DefaultRequestHeaders.Add("pragma", "no-cache");
      httpClient.DefaultRequestHeaders.Add("cache-control", "no-cache");
      httpClient.DefaultRequestHeaders.Add("dnt", "1");
      httpClient.DefaultRequestHeaders.Add("sec-fetch-site", "cross-site");
      httpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "no-cors");
      // httpClient.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
      httpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "script");
      httpClient.DefaultRequestHeaders.Add("accept",
        "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
      httpClient.DefaultRequestHeaders.Add("accept-language", "en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
      return httpClient;
    }

    private async ValueTask<bool> ConfirmCaptchaAsync(Uri pageUrl, string content, HttpClient httpClient,
      CancellationToken ct)
    {
      int attemptsCount = 0;
      var browsingContext = BrowsingContext.New(Configuration.Default);
      do
      {
        attemptsCount++;
        var captchaDoc = await browsingContext.OpenAsync(_ => _.Content(content), ct);
        var form = captchaDoc.Forms.FirstOrDefault(_ => _.Action.Contains(CaptchaConfirmPath))
                   ?? throw new InvalidOperationException("Can't find captcha confirm form");
        var siteKeyEl = form.QuerySelector(".g-recaptcha");
        var siteKey = siteKeyEl.Attributes["data-sitekey"].Value;
        var resolvedCaptchaToken =
          await _captchaSolveService.SolveReCaptchaV2Async(siteKey, pageUrl.ToString(), false, ct);
        var confirmCaptchaFields = form.Elements
          .Select(e => new KeyValuePair<string, string>(e.GetInputName(), e.GetInputValue()))
          .ToList();

        confirmCaptchaFields.Add(new KeyValuePair<string, string>("g-recaptcha-response", resolvedCaptchaToken));
        var confirmPayload = new FormUrlEncodedContent(confirmCaptchaFields);
        var confirmMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(form.Action))
        {
          Content = confirmPayload
        };

        var confirmResponse = await httpClient.SendAsync(confirmMessage, ct);
        var confirmContent = await confirmResponse.Content.ReadPossiblyGZippedAsStringAsync(ct);
        if (confirmContent.Contains(CaptchaConfirmPath))
        {
          content = confirmContent;
          continue;
        }

        var uValue = form.Elements["u"].GetInputValue();
        var idValue = form.Elements["id"].GetInputValue();
        var confirmedDoc = await browsingContext.OpenAsync(_ => _.Content(confirmContent), ct);
        var formEmailButtons = confirmedDoc.QuerySelectorAll(".formEmailButton");
        var expectedProfilePath = $"/profile/?u={uValue}&id={idValue}";

        return formEmailButtons.Any(b => b.Attributes["href"]?.Value.Contains(expectedProfilePath) == true);
      } while (attemptsCount < 10);

      throw new OperationCanceledException("Failed to submit form because of unable to solve captcha. Last response: "
                                           + content);
    }

    private class JsonSubmitResponse
    {
      [JsonProperty("result")] public string Result { get; set; }
      [JsonProperty("msg")] public string Message { get; set; }
      [JsonProperty("params")] public Dictionary<string, string> Params { get; set; }

      [JsonIgnore] public string Raw { get; set; }

      [JsonIgnore] public bool IsCaptchaError => Result == "error" && Message == "captcha";
      public bool IsSuccessful => Result == "success";
    }
  }
}