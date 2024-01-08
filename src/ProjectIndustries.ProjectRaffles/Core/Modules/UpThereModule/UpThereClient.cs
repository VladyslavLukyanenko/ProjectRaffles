using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.UpThereModule
{
  public class UpThereClient : ModuleHttpClientBase, IUpThereClient
  {
    private readonly ICountriesService _countriesService;
    private readonly ICaptchaSolveService _captchaSolver;
    private readonly CookieContainer _cookieContainer = new CookieContainer();

    public UpThereClient(ICountriesService countriesService, ICaptchaSolveService captchaSolver)
    {
      _countriesService = countriesService;
      _captchaSolver = captchaSolver;
    }

    protected override void ConfigureHttpClient(HttpClientOptions options)
    {
      options.AllowAutoRedirect = true;
      options.CookieContainer = _cookieContainer;
      options.PostConfigure = httpClient =>
      {
        httpClient.DefaultRequestHeaders.Add("User-Agent",
          "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.193 Safari/537.36");
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

    public async Task<string> LoginAsync(Account account, CancellationToken ct)
    {
      //try get cookies?
    //  await HttpClient.GetAsync("https://uptherestore.com/account", ct);
      var loginContent = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"form_type", "customer_login"},
        {"utf8", "✓"},
        {"customer[email]", account.Email},
        {"customer[password]", account.Password}
      });

      var LoginUrl = "https://uptherestore.com/account/login";

      var loginPost = await HttpClient.PostAsync(LoginUrl, loginContent, ct);
      var body = await loginPost.ReadStringResultOrFailAsync($"Can't login to the account with email {account.Email}", ct);
      if (body.Contains("challenge"))
      {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(body);
        
        var authToken = htmlDoc.DocumentNode.SelectSingleNode("//input[@name='authenticity_token']").GetAttributeValue("value","");

        var captcha =
          await _captchaSolver.SolveReCaptchaV2Async("6LeoeSkTAAAAAA9rkZs5oS82l69OEYjKRZAiKdaF", "https://uptherestore.com/challenge", true, ct);
        var postContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
          {"authenticity_token", authToken},
          {"g-recaptcha-response", captcha}
        });
             
        var lastPost = await HttpClient.PostAsync(LoginUrl, postContent, ct);
        if (!lastPost.IsSuccessStatusCode) await lastPost.FailWithRootCauseAsync("Failed to login", ct);

        return "";
      }

      return "";
    }

    public async Task<string> GetRaffleEndpointAsync(string raffleUrl, CancellationToken ct)
    {
      var raffleHtmlResponse = await HttpClient.GetAsync(raffleUrl, ct);
      var raffleHtml = await raffleHtmlResponse.ReadStringResultOrFailAsync("Can't access raffle page", ct);

      var htmlDoc = new HtmlDocument();
      htmlDoc.LoadHtml(raffleHtml);
      var formidhtml = htmlDoc.DocumentNode.SelectSingleNode("//iframe[@id='launchForm']").GetAttributeValue("src", "");
      var formId = formidhtml.Replace(" ", "%20");
      return formId;
    }

    public async Task<UpThereProduct> GetProductAsync(AddressFields addressFields, string raffleEndpoint,
      CancellationToken ct)
    {
      var getRaffle = await HttpClient.GetAsync(raffleEndpoint, ct);
      var finalHtml = await getRaffle.ReadStringResultOrFailAsync("Can't access form", ct);
      
      var raffleDoc = new HtmlDocument();
      raffleDoc.LoadHtml(finalHtml);

      var country = _countriesService.GetCountryName(addressFields.CountryId)
        .Replace(" (" + addressFields.CountryId + ")", "");

      //product
      var productHtml = raffleDoc.DocumentNode.SelectSingleNode("//input[@name='product']");
      var product = productHtml.GetAttributeValue("value", "");


      //calculate final price
      var priceHtml = raffleDoc.DocumentNode.SelectSingleNode("//span[@class='launch-form__total']");
      var productPrice = priceHtml.InnerText;
      var priceReplace = productPrice.Replace(" AUD plus shipping", "");
      var price = double.Parse(priceReplace, CultureInfo.InvariantCulture);
      var shipping = ShippingRate(country);
      var priceToConvert = price + shipping;
      var finalPrice = "$" + priceToConvert + ".00 AUD";

      return new UpThereProduct(product, finalPrice, country);
    }

    public async Task<bool> SubmitAsync(AddressFields addressFields, CreditCardFields creditCardFields, Account account,
      UpThereProduct product, string raffleEndpoint, string size, CancellationToken ct)
    {
      var state = _countriesService.GetProvinceName(addressFields.CountryId.Value,
        addressFields.ProvinceId.Value);

      var cardNoSpace = creditCardFields.Number.Value.Replace(" ", "");
      var cvv = creditCardFields.Cvv.Value.Replace("_", "");

      var raffleContent = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"fullname", addressFields.FullName.Value},
        {"email", account.Email},
        {"mobile", addressFields.PhoneNumber.Value},
        {"address", addressFields.AddressLine1.Value},
        {"city", addressFields.City.Value},
        {"state", state},
        {"postcode", addressFields.PostCode.Value},
        {"country", product.Country},
        {"term_marketing", "yes"},
        {"card[number]", cardNoSpace},
        {"card[month]", creditCardFields.Month.Value},
        {"card[year]", creditCardFields.Year.Value},
        {"card[cvc]", cvv},
        {"term_price", "yes"},
        {"term_terms", "yes"},
        {"product", product.Name},
        {"size", size},
        {"price", product.FinalPrice}
      });

      var message = new HttpRequestMessage(HttpMethod.Post, raffleEndpoint)
      {
        Content = raffleContent
      };

      var rafflePost = await HttpClient.SendAsync(message, ct);
      var rafflePostContent = await rafflePost.ReadStringResultOrFailAsync("Can't access site", ct);

      return rafflePostContent.Contains("Check your inbox to complete");
    }


    private static double ShippingRate(string country)
    {
      switch (country)
      {
        case "Australia":
          return 20;
        default:
          return 55;
      }
    }
  }
}