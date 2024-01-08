using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AmigoSkateshopModule
{
  public class AmigoSkateshopClient : ModuleHttpClientBase, IAmigoSkateshopClient
  {
    private readonly ICountriesService _countriesService;

    public AmigoSkateshopClient(ICountriesService countriesService)
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

    public async Task<string> GetProductAsync(string raffleurl, CancellationToken ct)
    {
      var getPage = await HttpClient.GetAsync(raffleurl, ct);
      if(!getPage.IsSuccessStatusCode) await getPage.FailWithRootCauseAsync(ct: ct);
      
      var body = await getPage.Content.ReadAsStringAsync(ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;
      var title = node.Replace("&ndash; Amigos Skate Shop", "").Replace("\n","").Replace("      ","").Replace("     ","");

      return title;
    }

    public async Task<AmigoSkateShopParsedForm> ParseFormAsync(string url, CancellationToken ct)
    {
      var getPage = await HttpClient.GetAsync(url, ct);
      var body = await getPage.ReadStringResultOrFailAsync("Can't access site", ct);

      var amigoFormPattern = @"content=""{formbuilder:\d{1,6}}"">";
      var amigoRegex = new Regex(amigoFormPattern);
      var amigoFormId = amigoRegex.Match(body).ToString();
      var formId = amigoFormId.Replace(@"content=""{formbuilder:", "").Replace(@"}"">", "");
      
      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;
      var title = node.Replace("&ndash;", @"-").Replace("\n","").Replace("      ","");

      return new AmigoSkateShopParsedForm(title, formId);
    }

    public async Task<bool> SubmitAsync(AmigoSkateshopSubmitPayload payload, CancellationToken ct)
    {
      var country = _countriesService.GetCountryName(payload.Profile.CountryId)
        .Replace(" (" + payload.Profile.CountryId + ")", "");

      var formSettings =
        @"{""name"":""Nombre/Name"",""email"":""Email"",""select"":""Talla/Size"",""text-4"":""Cuenta Instagram/Instagram Account"",""phone"":""Teléfono/Phone"",""country"":""País/Country"",""text"":""Código postal/Zip Code"",""text-3"":""Ciudad/City"",""text-2"":""Dirección/Address"",""text-5"":""Dirección/Address 2"",""checkbox"":""¿Cómo conseguir el zapato? How to get the shoe? ""}";

      var shipping = await Shipping(payload.Shipping);

      var fullName = payload.Profile.FirstName + " " + payload.Profile.LastName;

      var content = new MultipartFormDataContent("----WebKitFormBoundary" + Guid.NewGuid())
      {
        {new StringContent(fullName), "name"},
        {new StringContent(payload.Email), "email"},
        {new StringContent(payload.Size), "select"},
        {new StringContent($"@{payload.Instagram}"), "text-4"},
        {new StringContent(payload.Profile.PhoneNumber), "phone"},
        {new StringContent(country), "country"},
        {new StringContent(payload.Profile.PostCode), "text"},
        {new StringContent(payload.Profile.City), "text-3"},
        {new StringContent(payload.Profile.AddressLine1), "text-2"},
        {new StringContent($"{payload.Profile.AddressLine2}"), "text-5"},
        {new StringContent(shipping), "checkbox[]"},
        {new StringContent(payload.Captcha), "g-recaptcha-response"},
        {new StringContent(payload.Captcha), "reCaptcha"},
        {new StringContent(payload.ParsedForm.Title), "page[title]"},
        {new StringContent(payload.RaffleUrl), "page[href]"},
        {new StringContent(formSettings), "_keyLabel"}
      };


      var endpoint = $"https://form.globosoftware.net/api/front/form/{payload.ParsedForm.FormId}/send";

      var resp1 = await HttpClient.PostAsync(endpoint, content, ct);
      var respHtml = await resp1.ReadStringResultOrFailAsync("Error on submission", ct);

      return respHtml.Contains(@"""success"":""true""");
    }
    
    private Task<string> Shipping(string shippingType)
    {
      var shippingLowered = shippingType.ToLower();
      var result = shippingLowered switch
      {
        "online" => "Envío/Shipping",
        "instore" => "En tienda/In-Store",
        _ => throw new ArgumentOutOfRangeException(shippingLowered, "Shipping type not supported")
      };

      return Task.FromResult(result);
    }
  }
}