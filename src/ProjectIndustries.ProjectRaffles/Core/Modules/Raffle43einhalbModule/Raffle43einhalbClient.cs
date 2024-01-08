using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.Raffle43einhalbModule
{
  public class Raffle43einhalbClient : ModuleHttpClientBase, IRaffle43einhalbClient
  {
    private readonly ICountriesService _countriesService;
    private readonly CookieContainer _cookieContainer = new CookieContainer();

    public Raffle43einhalbClient(ICountriesService countriesService)
    {
      _countriesService = countriesService;
    }

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
    
    public async Task<string> GetProductNameAsync(string raffleurl, CancellationToken ct)
    {
      var getPage = await HttpClient.GetAsync(raffleurl, ct);
      var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;

      return node;
    }
    

    public async Task<string> ParseRaffleAsync(string raffleurl, string size, CancellationToken ct)
    {
      var getBody = await HttpClient.GetAsync(raffleurl, ct);
      var body = await getBody.ReadStringResultOrFailAsync("Can't get page", ct);

      var fullSizeRegexPattern = @"value=""\d{1,3}_\d{1,3}"" >\n * \("+size+" EUR";
      var fullSizeRegex = new Regex(fullSizeRegexPattern);
      var fullSizeRegexMatch = fullSizeRegex.Match(body).ToString();
            
      var sizeValue = new Regex(@"\d{1,3}_\d{1,3}").Match(fullSizeRegexMatch).ToString();
      
      var postContentForBsid = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"chosen_attribute_value", sizeValue},
        {"returnHtmlSnippets[partials][0][module]", "product"},
        {"returnHtmlSnippets[partials][0][path]", "_productDetail"},
        {"returnHtmlSnippets[partials][0][partialName]", "buybox"},
        {"returnHtmlSnippets[partials][0][params][template]", "default"}
      });

      HttpClient.DefaultRequestHeaders.Add("referer", raffleurl);
      HttpClient.DefaultRequestHeaders.Add("origin", "https://releases.43einhalb.com");
      HttpClient.DefaultRequestHeaders.Add("x-requested-with","XMLHttpRequest");
      var post = await HttpClient.PostAsync(raffleurl, postContentForBsid, ct);
      var postBody = await post.ReadStringResultOrFailAsync("Can't get product details", ct);

      HttpClient.DefaultRequestHeaders.Remove("referer");
      
      dynamic parsedBody = JObject.Parse(postBody);

      string bsid = parsedBody.initializedProduct.bsId;


      return bsid;
    }

    public async Task<bool> SubmitAsync(AddressFields addressFields, string housenumber, string email, string bsid, string captcha, CancellationToken ct)
    {
      var addressLine = addressFields.AddressLine1.Value;
      var street = addressLine.Replace(housenumber, "");

      var rnd = new Random();
      var salut = $"{rnd.Next(1, 2)}";
      var consent = @"Yes, I hereby accept the terms of participation. I confirm to receive emails from 43einhalb about this or similar products regularly. I agree with the privacy policy, which I can revoke any time at datenschutz@43einhalb.com";
      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"productBsId",bsid},
        {"salutation", salut},
        {"firstName",addressFields.FirstName.Value},
        {"lastName",addressFields.LastName.Value},
        {"email", email},
        {"paypalEmail", email},
        {"street", street},
        {"streetNr", housenumber},
        {"zipCode", addressFields.PostCode.Value},
        {"city", addressFields.City.Value},
        {"country", addressFields.CountryId.Value},
        {"consent", consent},
        {"gCaptchaToken", captcha}
      });

      HttpClient.DefaultRequestHeaders.Add("referer", $"https://releases.43einhalb.com/raffle-form?productBsId={bsid}");
      var endpoint = "https://releases.43einhalb.com/enter-raffle";
      var post = await HttpClient.PostAsync(endpoint, content, ct);
      if (!post.IsSuccessStatusCode) await post.FailWithRootCauseAsync("Can't submit entry", ct);
      return post.IsSuccessStatusCode;
    }
  }
}