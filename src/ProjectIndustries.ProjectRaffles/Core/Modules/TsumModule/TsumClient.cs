using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TsumModule
{
  public class TsumClient : ModuleHttpClientBase, ITsumClient
  {
    private readonly IBirthdayProviderService _birthdayProvider;

    public TsumClient(IBirthdayProviderService birthdayProvider)
    {
      _birthdayProvider = birthdayProvider;
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
      return node;
    }

    public async Task<TsumProductTags> ParseRaffleAsync(string raffleUrl, CancellationToken ct)
    {
      var removeLastChar = raffleUrl.Remove(raffleUrl.Length - 1);
      var apiUrl = removeLastChar.Replace("https://www.tsum.ru/lp/", "https://raffle.tsum.com/api/landings/");

      var getRaffle = await HttpClient.GetAsync(apiUrl, ct);
      var finalHtml = await getRaffle.ReadStringResultOrFailAsync("Can't access page", ct);

      dynamic json = JObject.Parse(finalHtml);
      var landingInt = (int) json["id"];
      var productInt = (int) json["products"][0]["id"];

      //var landingRelation = landingInt.ToString();
      //var productRelation = productInt.ToString();

      return new TsumProductTags(landingInt, productInt);
    }

    public async Task<bool> SubmitAsync(TsumSubmitPayload payload, CancellationToken ct)
    {
      var genderInt = Int32.Parse(payload.Gender);
      var tsumContent = new
      {
        surname = payload.Profile.LastName,
        name = payload.Profile.FirstName,
        age = payload.Age,
        phone = payload.Profile.PhoneNumber,
        gender = genderInt,
        email = payload.Email,
        loyalty_card = "",
        city = "",
        store = payload.PickupLocation,
        size = $"rn{payload.Size}",
        landingRelation = payload.ProductTags.LandingRelation,
        productRelation = payload.ProductTags.ProductRelation,
      };
      var customer = JsonConvert.SerializeObject(tsumContent);
      var content = new StringContent(customer, Encoding.UTF8, "application/json");

      var endpoint = "https://raffle.tsum.com/api/participants";
      var signup = await HttpClient.PostAsync(endpoint, content, ct);
      if(!signup.IsSuccessStatusCode) await signup.FailWithRootCauseAsync("Can't submit entry", ct);
      
      return signup.IsSuccessStatusCode;
    }
  }
}