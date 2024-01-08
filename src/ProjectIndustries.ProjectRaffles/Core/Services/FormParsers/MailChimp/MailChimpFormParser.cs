using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Html;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp
{
  public class MailChimpFormParser : IFormParser
  {
    private readonly IMailChimpHtmlFormParser _htmlFormParser;
    private readonly IMailChimpLandingPageFormParser _landingPageFormParser;

    public MailChimpFormParser(IMailChimpHtmlFormParser htmlFormParser,
      IMailChimpLandingPageFormParser landingPageFormParser)
    {
      _htmlFormParser = htmlFormParser;
      _landingPageFormParser = landingPageFormParser;
    }


    public bool IsModuleSupported(RaffleModuleType moduleType) => moduleType == RaffleModuleType.MailChimp;

    public async ValueTask<FormParseResult> ParseAsync(Uri url, CancellationToken ct = default)
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
      httpClient.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");
      httpClient.DefaultRequestHeaders.Add("accept",
        "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
      httpClient.DefaultRequestHeaders.Add("accept-language", "en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");

      var origin = url.GetLeftPart(UriPartial.Authority);
      httpClient.DefaultRequestHeaders.Add("origin", origin);
      // httpClient.DefaultRequestHeaders.Add("referer", url.ToString());

      var message = new HttpRequestMessage(HttpMethod.Get, url);
      var response = await httpClient.SendAsync(message, ct);

      string html = await response.Content.ReadPossiblyGZippedAsStringAsync(ct);
      var context = BrowsingContext.New();
      var doc = await context.OpenAsync(_ => _.Content(html), ct);

      var title = doc.QuerySelectorAll("h1,h2")
        .Select(_ => _.TextContent.Trim())
        .FirstOrDefault();
      var form = doc.Forms.OrderByDescending(_ => _.Elements.Length).FirstOrDefault();

      MailChimpParseResult parseResult;
      if (form != null)
      {
        parseResult = _htmlFormParser.Parse(form, cookieContainer);
      }
      else
      {
        string rawConfigUrl = doc.QuerySelector("[data-dojo-type='mojo/pages-signup-forms/Loader']")
          ?.Attributes["endpoint"].Value;
        if (string.IsNullOrWhiteSpace(rawConfigUrl))
        {
          throw new InvalidOperationException("No forms found on page");
        }

        var configUrl = new Uri(rawConfigUrl);
        parseResult = await _landingPageFormParser.ParseAsync(configUrl, httpClient, ct);
      }
      //
      // httpClient.DefaultRequestHeaders.Add("sec-fetch-site", "cross-site");
      // httpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
      // httpClient.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
      // httpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
      // httpClient.DefaultRequestHeaders.Add("sec-gpc", "1");
      //
      // var origin = url.GetLeftPart(UriPartial.Authority);
      // httpClient.DefaultRequestHeaders.Add("origin", origin);
      // httpClient.DefaultRequestHeaders.Add("referer", url.ToString());

      return new MailChimpFormParseResult(title, url, parseResult.Fields, html, parseResult.SubmitHandler);
    }
  }
}