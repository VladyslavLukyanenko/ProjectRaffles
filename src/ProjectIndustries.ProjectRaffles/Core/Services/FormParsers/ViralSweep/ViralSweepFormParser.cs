using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep.FieldFactories;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep
{
  public class ViralSweepFormParser : IFormParser
  {
    private static readonly Regex FidRegex =
      new(@"https://app.viralsweep.com/vsa-(\w+)-(\w{6}-\w{5}).js", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex DirectRegex = new(@"https://app.viralsweep.com/sweeps/(\w+)/(\w{6}-\w{5})",
      RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly IEnumerable<IFieldFromHtmlFieldFactory> _fieldFactories;

    public ViralSweepFormParser(IEnumerable<IFieldFromHtmlFieldFactory> fieldFactories)
    {
      _fieldFactories = fieldFactories;
    }

    public bool IsModuleSupported(RaffleModuleType moduleType) => moduleType == RaffleModuleType.Viralsweep;

    public async ValueTask<FormParseResult> ParseAsync(Uri url, CancellationToken ct = default)
    {
      var httpClient = CreateHttpClient();
      var ctx = BrowsingContext.New();
      var formResponse = await FetchFormHtmlAsync(url, httpClient, ctx, ct);
      var formDoc = await ctx.OpenAsync(_ => _.Content(formResponse.formHtml), ct);
      string invalidGeoWarning = null;
      var invalidGeoElement = formDoc.QuerySelector(".invalid-geo");
      if (invalidGeoElement != null)
      {
        invalidGeoWarning = invalidGeoElement.TextContent.Trim();
      }

      var form = formDoc.Forms.FirstOrDefault(_ => _.Id == "entry-form")
                 ?? throw new InvalidOperationException("Can't find form");

      var resolvedFieldNames = new List<string>();
      var fields = GetFields(form, resolvedFieldNames).ToArray();

      var entrySource = fields.FirstOrDefault(_ => _.SystemName == "entry_source");
      if (entrySource != null)
      {
        entrySource.Value = url.GetLeftPart(UriPartial.Authority);
      }

      var title = formDoc.QuerySelector("h1.title").TextContent.Trim();
      var captchaElement = form.QuerySelector(".captcha_wrapper");
      string siteKey = null;
      if (captchaElement != null)
      {
        siteKey = captchaElement.QuerySelector(".g-recaptcha")
          .Attributes["data-sitekey"]
          .Value;
      }

      return new ViralSweepParseResult(title, url, fields, formResponse.formHtml, siteKey,
        new ViralSweepSubmitHandler(httpClient, form.Method), formResponse.formUrl, invalidGeoWarning);
    }

    private static HttpClient CreateHttpClient()
    {
      var cookieContainer = new CookieContainer();
      var handler = new HttpClientHandler
      {
        UseCookies = true,
        CookieContainer = cookieContainer
      };

      var httpClient = new HttpClient(handler);
      httpClient.DefaultRequestHeaders.Add("User-Agent",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
      httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
      httpClient.DefaultRequestHeaders.Add("sec-fetch-site", "none");
      httpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
      httpClient.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
      httpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
      httpClient.DefaultRequestHeaders.Add("pragma", "no-cache");
      httpClient.DefaultRequestHeaders.Add("cache-control", "no-cache");
      httpClient.DefaultRequestHeaders.Add("accept-encoding", "gzip");
      httpClient.DefaultRequestHeaders.Add("accept",
        "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
      httpClient.DefaultRequestHeaders.Add("accept-language", "en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
      return httpClient;
    }

    private static async ValueTask<(string formHtml, string formUrl)> FetchFormHtmlAsync(Uri url, HttpClient httpClient,
      IBrowsingContext ctx,
      CancellationToken ct)
    {
      var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
      var responseMessage = await httpClient.SendAsync(requestMessage, ct);
      var rootPage = await responseMessage.Content.ReadPossiblyGZippedAsStringAsync(ct);

      var directMatches = DirectRegex.Match(url.ToString());
      if (directMatches.Success)
      {
        return (rootPage, url.ToString());
      }

      var doc = await ctx.OpenAsync(_ => _.Content(rootPage), ct);
      var iframeLoaderScript = (IHtmlScriptElement) doc.QuerySelector("[src^='https://app.viralsweep.com/vsa-']");

      var refrerEncoded = url.ToString().UriDataEscape();
      var fidMatch = FidRegex.Match(iframeLoaderScript.Source);
      var formType = fidMatch.Groups[1].Value;
      var fid = fidMatch.Groups[2].Value;

      var formUrl =
        $"https://app.viralsweep.com/vrlswp/{formType}/{fid}?framed=1&vs_eid_hash=&ref={refrerEncoded}&hash=";
      httpClient.DefaultRequestHeaders.Referrer = new Uri(formUrl);
      httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

      var formRequest = new HttpRequestMessage(HttpMethod.Get, formUrl);
      var formResponse = await httpClient.SendAsync(formRequest, ct);
      var formHtml = await formResponse.Content.ReadPossiblyGZippedAsStringAsync(ct);
      return (formHtml, formUrl);
    }

    private IEnumerable<Field> GetFields(IHtmlFormElement form, IList<string> resolvedFieldNames)
    {
      var publicFields = form.QuerySelectorAll(".tep,.stripe_payment_input_form").OfType<IHtmlElement>();
      foreach (IHtmlElement fieldGroup in publicFields)
      {
        var factory = _fieldFactories.FirstOrDefault(_ => _.CanHandle(fieldGroup.ClassList));
        if (factory == null)
        {
          continue;
        }

        foreach (var field in factory.Create(fieldGroup))
        {
          resolvedFieldNames.Add(field.SystemName);
          yield return field;
        }
      }

      foreach (var input in form.Elements.OfType<IHtmlInputElement>()
        .Where(_ => !string.IsNullOrEmpty(_.Name) && !resolvedFieldNames.Contains(_.Name)))
      {
        yield return new HiddenField(input.Name, input.Value);
      }
    }
  }
}