using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Apm.Api;
using Newtonsoft.Json.Linq;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NakedCphModule
{
  public class NakedCphClient : ModuleHttpClientBase, INakedCphClient
  {
    protected override void ConfigureHttpClient(HttpClientOptions options)
    {
      options.AllowAutoRedirect = false;
      options.PostConfigure = httpClient =>
      {
        httpClient.DefaultRequestHeaders.Add("User-Agent",
          "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        httpClient.DefaultRequestHeaders.Add("dnt", "1");
        httpClient.DefaultRequestHeaders.Add("accept-language","en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
        httpClient.DefaultRequestHeaders.Add("sec-fetch-dest","document");
        httpClient.DefaultRequestHeaders.Add("sec-fetch-mode","navigate");
        httpClient.DefaultRequestHeaders.Add("sec-fetch-site","cross-site");
        httpClient.DefaultRequestHeaders.Add("sec-gpc","1");
      };
    }

    public async Task<string> GetProductAsync(string raffleurl, CancellationToken ct)
    {
      var customFieldsUrl = "https://young-chamber-15493.herokuapp.com/fields";
      var getRaffle = await HttpClient.GetAsync(customFieldsUrl, ct);
      var finalHtml = await getRaffle.ReadStringResultOrFailAsync("Can't access raffle details", ct);

      var customApiParse = JObject.Parse(finalHtml);

      string product = (string) customApiParse[raffleurl]["raffle"];

      return product;
    }

    public async Task<string> GetIPAsync()
    {
      var ipresponse = await HttpClient.GetAsync("http://bot.whatismyipaddress.com/");
      var publicIp = await ipresponse.ReadStringResultOrFailAsync("Can't get IP");

      return publicIp;
    }

    public async Task<NakedCphProductTags> GetRaffleTags(string raffleurl, CancellationToken ct)
    {
      var customFieldsUrl = "https://young-chamber-15493.herokuapp.com/fields";
      var getRaffle = await HttpClient.GetAsync(customFieldsUrl, ct);
      var finalHtml = await getRaffle.ReadStringResultOrFailAsync("Can't access raffle details", ct);

      var customApiParse = JObject.Parse(finalHtml);
      string tags = (string) customApiParse[raffleurl]["tags"];
      string token = (string) customApiParse[raffleurl]["token"];

      //todo: cloudflare cookies needed for getting the page
      //the following is commented out, as it doesn't work without cloudflare cookies. If we solve cloudflare, following will work
      /*
      var getRaffle = await _httpClient.GetAsync(raffleurl, ct);
      var finalHtml = await getRaffle.Content.ReadAsStringAsync();
      var raffleDoc = new HtmlDocument();
      raffleDoc.LoadHtml(finalHtml);

      //tags
      var tagsHtml = raffleDoc.DocumentNode.SelectSingleNode("//input[@name='tags[]']");
      var tags = tagsHtml.GetAttributeValue("value", "");

      //token
      var tokenHtml = raffleDoc.DocumentNode.SelectSingleNode("//input[@name='token']");
      var token = tokenHtml.GetAttributeValue("value", ""); */

      return new NakedCphProductTags(tags, token);
    }

    public async Task<bool> SubmitAsync(NakedCphSubmitPayload payload, CancellationToken ct)
    {
      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"tags[]", payload.ProductTags},
        {"token", "c812c1ff-2a5a0fe-efad139-d754416-71e1e60-2ce"},
        {"rule_email", payload.Account.Email},
        {"fields[Raffle.Instagram Handle]", payload.Instagram},
        {"fields[Raffle.Phone Number]", payload.Profile.PhoneNumber.Value},
        {"fields[Raffle.First Name]", payload.Profile.FirstName.Value},
        {"fields[Raffle.Last Name]", payload.Profile.LastName.Value},
        {"fields[Raffle.Shipping Address]", payload.Profile.AddressLine1.Value},
        {"fields[Raffle.Postal Code]", payload.Profile.PostCode.Value},
        {"fields[Raffle.City]", payload.Profile.City.Value},
        {"fields[Raffle.Country]", payload.Profile.CountryId.Value}, //2 digits
        {"fields[SignupSource.ip]", payload.IP},
        {"fields[SignupSource.useragent]", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36"},
        {"language", "sv"},
        {"g-recaptcha-response", payload.Captcha}
      });

      HttpClient.DefaultRequestHeaders.Add("origin","https://www.nakedcph.com");
      HttpClient.DefaultRequestHeaders.Add("referer","https://www.nakedcph.com/");
      var url = "https://app.rule.io/subscriber-form/subscriber";
      var signup = await HttpClient.PostAsync(url, content, ct);
      
      string headerLocation = "";
      var headers = signup.Headers;
      IEnumerable<string> values;
      if (headers.TryGetValues("location", out values))
      { 
        headerLocation = values.First();
      }

      if (!headerLocation.Contains("was-successful")) await signup.FailWithRootCauseAsync("Error on submission", ct);
      
     // if(!signupContent.Contains("YOUR REGISTRATION WAS SUCCESSFUL")) await signup.FailWithRootCauseAsync("Submission error", ct);
      return headerLocation.Contains("was-successful");
    }
  }
}