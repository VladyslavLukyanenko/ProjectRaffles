using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bogus.Extensions;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.MahaModule
{
  public class MahaClient : ModuleHttpClientBase, IMahaClient
  {
    private readonly ICountriesService _countriesService;

    public MahaClient(ICountriesService countriesService)
    {
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

    public async Task<string> FetchProductAsync(string raffleUrl, CancellationToken ct)
    {
      var getPage = await HttpClient.GetAsync(raffleUrl, ct);
      var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var product = doc.DocumentNode.SelectSingleNode("//input[@name='product']").GetAttributeValue("value", "");
      return product;
    }

    public async Task<MahaParsedRaffle> ParseRaffleAsync(string url, CancellationToken ct)
    {
      var getPage = await HttpClient.GetAsync(url, ct);
      var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var product = doc.DocumentNode.SelectSingleNode("//input[@name='product']").GetAttributeValue("value", "");
      var productImage = doc.DocumentNode.SelectSingleNode("//input[@name='productImage']")
        .GetAttributeValue("value", "");
      var productId = doc.DocumentNode.SelectSingleNode("//input[@name='productId']").GetAttributeValue("value", "");

      return new MahaParsedRaffle(product, productId, productImage);
    }


    public async Task<bool> SubmitAsync(MahaSubmitPayload payload, CancellationToken ct)
    {
      var country = _countriesService.GetCountryName(payload.Profile.CountryId)
        .Replace(" (" + payload.Profile.CountryId + ")", "");
      ;

      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"product", payload.ParsedRaffle.Product},
        {"productImage", payload.ParsedRaffle.ProductImage},
        {"productId", payload.ParsedRaffle.ProductId},
        {"dontfill", ""},
        {"g-recaptcha-response", payload.Captcha},
        {"firstname", payload.Profile.FirstName.Value},
        {"lastname", payload.Profile.LastName.Value},
        {"phone", payload.Profile.PhoneNumber.Value},
        {"shoeSize", payload.Size},
        {"email", payload.Account.Email},
        {"country", country},
        {"instagram", payload.Instagram},
        {"shipping", payload.Shipping},
        {"keepMePosted", "on"}
      });


      var endpoint = "https://apps.shopmonkey.nl/maha/raffle/raffle.php";
      var raffleresponse = await HttpClient.PostAsync(endpoint, content, ct);
      if(!raffleresponse.IsSuccessStatusCode) await raffleresponse.FailWithRootCauseAsync("Error on submission", ct);

      return raffleresponse.IsSuccessStatusCode;
    }
  }
}