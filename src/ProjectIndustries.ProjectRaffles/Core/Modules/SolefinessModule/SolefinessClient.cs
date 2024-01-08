using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SolefinessModule
{
  public class SolefinessClient : ModuleHttpClientBase, ISolefinessClient
  {
    private readonly ICountriesService _countriesService;
    private readonly CookieContainer _cookieContainer = new CookieContainer();

    public SolefinessClient(ICountriesService countriesService)
    {
      _countriesService = countriesService;
    }

    protected override void ConfigureHttpClient(HttpClientOptions options)
    {
      options.AllowAutoRedirect = true;
      options.CookieContainer = _cookieContainer;
      options.PostConfigure = httpClient =>
      {
        httpClient.DefaultRequestHeaders.Add("User-Agent",
          "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        httpClient.DefaultRequestHeaders.Add("dnt", "1");
      };
    }
    
    public async Task<string> GetProductAsync(string raffleUrl, CancellationToken ct)
    {
      var getPage = await HttpClient.GetAsync(raffleUrl, ct);
      var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;
      return node;
    }

    public async Task<SolefinessParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct)
    {
      var raffleGet = await HttpClient.GetAsync(raffleUrl, ct);
      var raffleContent = await raffleGet.ReadStringResultOrFailAsync("Can't access page", ct);
            
      var doc = new HtmlDocument();
      doc.LoadHtml(raffleContent);

      var iFrameId = doc.DocumentNode.SelectSingleNode("//div[@class='pxFormGenerator']").GetAttributeValue("id", "");
      var iFrame = "https://formbuilder.hulkapps.com/corepage/customform?id=" + iFrameId;

      var getFrame = await HttpClient.GetAsync(iFrame, ct);
      var frameContent = await getFrame.ReadStringResultOrFailAsync("Can't access form", ct);

      var termsRegex = @"""I have read and .*"" c";
      var findTerms = new Regex(termsRegex);
      var termsMatch = findTerms.Match(frameContent).ToString();
      var terms = termsMatch.Replace(@""" c", "").Replace(@"""","");
      
      return new SolefinessParsed(iFrameId, terms);
    }

    public Task<string> CraftContentAsync(AddressFields profile, CreditCardFields creditcard, SolefinessParsed parsed, string email, string instagram, string size)
    {
      var country = _countriesService.GetCountryName(profile.CountryId)
        .Replace(" (" + profile.CountryId + ")", "");

      var settings = new JsonSerializerSettings
      {
        ContractResolver = new DefaultContractResolver()
      };

      var craftedContent = new SolefinessJson
      {
        FirstName = profile.FirstName.Value,
        LastName = profile.LastName.Value,
        Email = email,
        Instagram = instagram,
        Address = profile.AddressLine1.Value,
        PostCode = profile.PostCode.Value,
        Country = country,
        PhoneNumber = profile.PhoneNumber.Value,
        Size = size,
        CreditCardName = profile.FirstName.Value + " " + profile.LastName.Value,
        CreditCardNumber = creditcard.Number.Value,
        CreditCardExpiration = $"{creditcard.Month.Value}/{creditcard.Year.Value}",
        CreditCardCvv = creditcard.Cvv,
        TermsString = "On"
      };
      var json = JsonConvert.SerializeObject(craftedContent, settings);
      var replacedJson = json.Replace("TermsString", parsed.Terms);

      return Task.FromResult(replacedJson);
    }

    public async Task<bool> SubmitAsync(SolefinessParsed parsed, string jsonContent, CancellationToken ct)
    {
      var raffleContent = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"form_uuid", parsed.Id},
        {"formResponse", jsonContent},
        {"confirmationMail", ""},
        {"is_pro", "false"}
      });

      var endpoint = "https://formbuilder.hulkapps.com/ajaxcall/formresponse";
      HttpClient.DefaultRequestHeaders.Add("referer",$"https://formbuilder.hulkapps.com/corepage/customform?id={parsed.Id}");
      
      var rafflePost = await HttpClient.PostAsync(endpoint, raffleContent, ct);
      if(!rafflePost.IsSuccessStatusCode) await rafflePost.FailWithRootCauseAsync("Can't submit entry", ct);

      return rafflePost.IsSuccessStatusCode;
    }
  }
}