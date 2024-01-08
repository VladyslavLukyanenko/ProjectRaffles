using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TresBienModule
{
  public class TresBienClient : ModuleHttpClientBase, ITresBienClient
  {
    private readonly CookieContainer _cookieContainer = new CookieContainer();
    private readonly ICountriesService _countriesService;

    public TresBienClient(ICountriesService countriesService)
    {
      _countriesService = countriesService;
    }

    protected override void ConfigureHttpClient(HttpClientOptions options)
    {
      options.CookieContainer = _cookieContainer;
      options.PostConfigure = httpClient =>
      {
        httpClient.DefaultRequestHeaders.Add("User-Agent",
          "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        httpClient.DefaultRequestHeaders.Add("dnt", "1");
      };
    }

    public async Task<TresBienProductTags> GetRaffleTags(string raffleurl, CancellationToken ct)
    {
      var getRaffle = await HttpClient.GetAsync(raffleurl, ct);
      var finalHtml = await getRaffle.ReadStringResultOrFailAsync("Can't access page", ct);
      
      var raffleDoc = new HtmlDocument();
      raffleDoc.LoadHtml(finalHtml);

      //tags
      var formkeyHtml = raffleDoc.DocumentNode.SelectSingleNode("//input[@name='form_key']");
      var formkey = formkeyHtml.GetAttributeValue("value", "");

      //token
      var skuHtml = raffleDoc.DocumentNode.SelectSingleNode("//input[@name='sku']");
      var sku = skuHtml.GetAttributeValue("value", "");

      return new TresBienProductTags(formkey, sku);
    }

    public async Task<bool> SubmitAsync(TresBienSubmitPayload payload, CancellationToken ct)
    {
      var formKeyCookie = new Cookie("form_key", payload.ProductTags.FormKey) {Domain = "tres-bien.com"};
      _cookieContainer.Add(formKeyCookie);

      var size = payload.SizeValue.Replace(" ", "_");
      var country = _countriesService.GetCountryName(payload.Profile.CountryId)
        .Replace(" (" + payload.Profile.CountryId + ")", "");

      var fullName = payload.Profile.FirstName + " " + payload.Profile.LastName;
      var content = new MultipartFormDataContent("----WebKitFormBoundary" + Guid.NewGuid())
      {
        {new StringContent(payload.ProductTags.FormKey), "form_key"},
        {new StringContent(payload.ProductTags.SKU), "sku"},
        {new StringContent(fullName), "fullname"},
        {new StringContent(payload.Email), "email"},
        {new StringContent(payload.Profile.AddressLine1.Value), "address"},
        {new StringContent(payload.Profile.PostCode.Value), "zipcode"},
        {new StringContent(payload.Profile.City.Value), "city"},
        {new StringContent(country), "country"},
        {new StringContent(payload.Profile.PhoneNumber.Value), "phone"},
        {new StringContent(size), "Size_raffle"},
        {new StringContent(payload.Captcha), "g-recaptcha-response"}
      };

      var url = "https://tres-bien.com/tbscatalog/manage/rafflepost/";
      var signup = await HttpClient.PostAsync(url, content, ct);
      if(!signup.IsSuccessStatusCode) await signup.FailWithRootCauseAsync("Can't submit entry", ct);
      
      return signup.IsSuccessStatusCode;
    }
  }
}