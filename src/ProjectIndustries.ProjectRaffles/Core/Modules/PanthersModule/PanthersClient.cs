using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PanthersModule
{
  public class PanthersClient : ModuleHttpClientBase, IPanthersClient
  {
    private readonly ICountriesService _countriesService;
    private readonly IBirthdayProviderService _birthdayProvider;

    public PanthersClient(ICountriesService countriesService, IBirthdayProviderService birthdayProvider)
    {
      _birthdayProvider = birthdayProvider;
      _countriesService = countriesService;
    }

    protected override void ConfigureHttpClient(HttpClientOptions options)
    {
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
      var title = node.Replace("&quot;", @"""").Replace(" - Panthers Store", "");

      return title;
    }

    public async Task<PanthersParsedRaffle> ParseRaffleAsync(string raffleurl, CancellationToken ct)
    {
      var getPage = await HttpClient.GetAsync(raffleurl, ct);
      var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var product = doc.DocumentNode.SelectSingleNode("//input[@name='product']").GetAttributeValue("value", "");

      var image = doc.DocumentNode.SelectSingleNode("//input[@name='productImage']").GetAttributeValue("value", "");

      var id = doc.DocumentNode.SelectSingleNode("//input[@name='productId']").GetAttributeValue("value", "");

      var store = doc.DocumentNode.SelectSingleNode("//input[@name='shop']").GetAttributeValue("value", "");

      return new PanthersParsedRaffle(product, image, id, store);
    }
    

    public async Task<bool> SubmitAsync(PanthersSubmitPayload payload, CancellationToken ct)
    {
      var age = await _birthdayProvider.GenerateAge();
      
      var country = _countriesService.GetCountryName(payload.Profile.CountryId)
        .Replace(" (" + payload.Profile.CountryId + ")", "");
      
      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"product", payload.ParsedRaffle.Product},
        {"productImage", payload.ParsedRaffle.Image},
        {"productId", payload.ParsedRaffle.Id},
        {"shop", payload.ParsedRaffle.Store},
        {"dontfill", ""},
        {"firstname", payload.Profile.FirstName},
        {"lastname", payload.Profile.LastName},
        {"age", $"{age}"},
        {"gender", "Male"},
        {"shoeSize", payload.SizeValue}, //2 digits
        {"address", payload.Profile.AddressLine1},
        {"zip", payload.Profile.PostCode},
        {"country", country},
        {"email", payload.Account.Email},
        {"phone", payload.Profile.PhoneNumber},
        {"instagram", payload.InstagramHandle}
      });

      var endpoint = "https://apps.shopmonkey.nl/panthers/raffle/raffle.php";
      var signup = await HttpClient.PostAsync(endpoint, content, ct);
      var response = await signup.ReadStringResultOrFailAsync("Error on submission", ct);
      
      if(!response.Contains("success")) await signup.FailWithRootCauseAsync("Error on entry", ct);
      
      return response.Contains("success");
    }
  }
}