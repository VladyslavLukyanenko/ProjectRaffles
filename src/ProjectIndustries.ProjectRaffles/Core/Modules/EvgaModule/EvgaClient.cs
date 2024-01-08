using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.EvgaModule
{
  public class EvgaClient : ModuleHttpClientBase, IEvgaClient
  {
    private readonly CookieContainer _cookieContainer = new CookieContainer();

    protected override void ConfigureHttpClient(HttpClientOptions options)
    {
      options.AllowAutoRedirect = false;
      options.CookieContainer = _cookieContainer;
      options.PostConfigure = httpClient =>
      {
        httpClient.DefaultRequestHeaders.Add("User-Agent",
          "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        httpClient.DefaultRequestHeaders.Add("dnt", "1");
      };
    }

    public async Task<string> GetProductAsync(string raffleurl, CancellationToken ct)
    {
      var getPage = await HttpClient.GetAsync(raffleurl, ct);
      if(!getPage.IsSuccessStatusCode) await getPage.FailWithRootCauseAsync("Can't access site", ct);
      
      var body = await getPage.Content.ReadAsStringAsync(ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;
      var title = node.Replace("EVGA - Products - ", "").Replace("\n", "");

      return title;
    }

    public async Task<EvgaParsedFields> ParseProductAsync(string url, CancellationToken ct)
    {
      var getBody = await HttpClient.GetAsync(url, ct);
      if(!getBody.IsSuccessStatusCode) await getBody.FailWithRootCauseAsync("Can't access site", ct);
      
      var body = await getBody.Content.ReadAsStringAsync(ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var viewState = doc.DocumentNode.SelectSingleNode("//input[@name='__VIEWSTATE']")
        .GetAttributeValue("value", "");

      var viewStateGenerator = doc.DocumentNode.SelectSingleNode("//input[@name='__VIEWSTATEGENERATOR']")
        .GetAttributeValue("value", "");

      var eventValidation = doc.DocumentNode.SelectSingleNode("//input[@name='__EVENTVALIDATION']")
        .GetAttributeValue("value", "");

      return new EvgaParsedFields(viewState, viewStateGenerator, eventValidation);
    }

    public async Task<bool> SubmitAsync(AddressFields profile, Account account, string url, EvgaParsedFields parsedFields, string captcha,
      CancellationToken ct)
    {
      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {
          "RadScriptManager1_TSM",
          ";;System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35:en-US:2d39c544-8ec0-4a2c-bc21-04e23af02570:ea597d4b:b25378d2"
        },
        {"__EVENTTARGET", ""},
        {"__EVENTARGUMENT", ""},
        {"__VIEWSTATE", parsedFields.ViewState},
        {"__VIEWSTATEGENERATOR", parsedFields.ViewStateGenerator},
        {"__EVENTVALIDATION", parsedFields.EventValidation},
        {"tbFirstName", profile.FirstName.Value},
        {"tbLastName", profile.LastName.Value},
        {"tbEmail", account.Email},
        {"btnSubmit", "Submit"},
        {"g-recaptcha-response", captcha}
      });

      var finalizeRegistration = await HttpClient.PostAsync(url, content, ct);
      if(!finalizeRegistration.IsSuccessStatusCode) await finalizeRegistration.FailWithRootCauseAsync("Can't submit", ct);
      
      var finalBody = await finalizeRegistration.Content.ReadAsStringAsync(ct);

      return finalBody.Contains("Thank you for your product alert subscription");
    }
  }
}