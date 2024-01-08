// #define USE_JSON

#define DONT_USE_JSON
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Html
{
  public class MailChimpHtmlFormSubmitHandler : IFormSubmitHandler
  {
    private const string CaptchaConfirmPath = "/subscribe/confirm-captcha";
    private readonly IMailchimpService _mailchimpService;
    private readonly CookieContainer _cookieContainer;
    private readonly ICaptchaSolveService _captchaSolveService;

    private readonly Uri _submitUri;
    private readonly string _submitMethod;

    public MailChimpHtmlFormSubmitHandler(IMailchimpService mailchimpService, CookieContainer cookieContainer,
      Uri submitUri, string submitMethod, ICaptchaSolveService captchaSolveService)
    {
      _mailchimpService = mailchimpService;
      _cookieContainer = cookieContainer;
      _submitUri = submitUri;
      _submitMethod = submitMethod;
      _captchaSolveService = captchaSolveService;
    }

    public async ValueTask<FormSubmitResult> SubmitAsync(Uri pageUrl, IEnumerable<Field> fields,
      CancellationToken ct = default)
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
#if USE_JSON
      httpClient.DefaultRequestHeaders.Add("pragma", "no-cache");
      httpClient.DefaultRequestHeaders.Add("cache-control", "no-cache");
      httpClient.DefaultRequestHeaders.Add("dnt", "1");
#else

      httpClient.DefaultRequestHeaders.Add("cache-control", "max-age=0");
#endif
      httpClient.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
      httpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
      httpClient.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
      httpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
      httpClient.DefaultRequestHeaders.Add("accept",
        "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
      httpClient.DefaultRequestHeaders.Add("accept-language", "en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
      var submitFields = fields.ToList();

#if USE_JSON
      var timestamp = await _mailchimpService.GetUnixTime();
      submitFields.Add(new HiddenField("_", timestamp));
#endif
      var origin = pageUrl.GetLeftPart(UriPartial.Authority);
      httpClient.DefaultRequestHeaders.Add("origin", origin);
      httpClient.DefaultRequestHeaders.Add("referer", pageUrl.ToString());

#if USE_JSON
      var jqueryId = await _mailchimpService.GeneratejQueryId();
      submitFields.Insert(0, new HiddenField("c", jqueryId));
#endif

      var queryParams = submitFields.Where(f => !(f is CheckboxField cb) || cb.IsChecked)
        .Select(_ => new KeyValuePair<string, string>(_.SystemName, _.Value?.ToString()))
        .ToList();

#if !USE_JSON
      var submitPayload = new FormUrlEncodedContent(queryParams);
#endif

      var requestUri = _submitUri;
#if USE_JSON
      var query = string.Join("&", queryParams.Select(p => $"{p.Key}={p.Value}"));
      var submitUrlBuilder = new UriBuilder(_submitUri) {Query = query};
      requestUri = submitUrlBuilder.Uri;
#endif
      var message = new HttpRequestMessage(new HttpMethod(_submitMethod), requestUri)
#if !USE_JSON
        {
          Content = submitPayload
        }
#endif
        ;

      var postAccount = await httpClient.SendAsync(message, ct);
      string content = await postAccount.Content.ReadPossiblyGZippedAsStringAsync(ct);

      var result = content.Contains(CaptchaConfirmPath)
        ? await ConfirmCaptchaAsync(pageUrl, content, httpClient, ct)
        : content.Contains("success") || content.Contains("check your email")
                                      || content.Contains("please click the link in the email");

      return result
        ? FormSubmitResult.Successful()
        : FormSubmitResult.Failed(content);
    }

    private async Task<bool> ConfirmCaptchaAsync(Uri pageUrl, string content, HttpClient httpClient,
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
  }
}