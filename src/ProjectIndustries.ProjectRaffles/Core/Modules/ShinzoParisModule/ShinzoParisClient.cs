using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.ShinzoParisModule
{
  public class ShinzoParisClient : ModuleHttpClientBase, IShinzoParisClient
  {
    private readonly ICountriesService _countriesService;

    public ShinzoParisClient(ICountriesService countriesService)
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
      var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;
      var title = node.Replace(" | SHINZO Paris, sneakers Raffle", "");

      return title;
    }

    public async Task<ShinzoParisParsedRaffle> ParseRaffleAsync(string raffleUrl, CancellationToken ct)
    {
      var getRaffle = await HttpClient.GetAsync(raffleUrl, ct);
      var finalHtml = await getRaffle.ReadStringResultOrFailAsync("Can't access page", ct);
      
      var raffleDoc = new HtmlDocument();
      raffleDoc.LoadHtml(finalHtml);

      var wpfc7 = raffleDoc.DocumentNode.SelectSingleNode("//input[@name='_wpcf7']").GetAttributeValue("value", "");
      var wpfc7version = raffleDoc.DocumentNode.SelectSingleNode("//input[@name='_wpcf7_version']")
        .GetAttributeValue("value", "");
      var locale = raffleDoc.DocumentNode.SelectSingleNode("//input[@name='_wpcf7_locale']")
        .GetAttributeValue("value", "");
      var unitTag = raffleDoc.DocumentNode.SelectSingleNode("//input[@name='_wpcf7_unit_tag']")
        .GetAttributeValue("value", "");
      var container = raffleDoc.DocumentNode.SelectSingleNode("//input[@name='_wpcf7_container_post']")
        .GetAttributeValue("value", "");
      var options = raffleDoc.DocumentNode.SelectSingleNode("//input[@name='_wpcf7cf_options']")
        .GetAttributeValue("value", "").Replace("&quot;", @"""");

      dynamic groupObject = JObject.Parse(options);
      string group = groupObject.conditions[0].then_field;

     var raffleRegexPattern = @"raffle.*-wrap"" style=""display:none !important";
      var raffleRegex = new Regex(raffleRegexPattern);
      var matchRegex = raffleRegex.Match(finalHtml).ToString();
      var raffleId = matchRegex.Replace(@"-wrap"" style=""display:none !important", "");

      return new ShinzoParisParsedRaffle(wpfc7, wpfc7version, locale, unitTag, container, options, group, raffleId);
    }

    public async Task<bool> SubmitAsync(ShinzoParisSubmitPayload payload, CancellationToken ct)
    {
      var content = await CreateContent(payload);

      var endpoint =
        $"https://raffle.shinzo.paris/en/wp-json/contact-form-7/v1/contact-forms/{payload.ParsedRaffle.Wpfc7}/feedback";
      var signup = await HttpClient.PostAsync(endpoint, content, ct);
      var signupHtml = await signup.ReadStringResultOrFailAsync("Error on submission", ct);
      
      if(signupHtml.Contains("spam")) await signup.FailWithRootCauseAsync("Entry marked as spam", ct);

      return signupHtml.Contains("succeeded");
    }

    private Task<MultipartFormDataContent> CreateContent(ShinzoParisSubmitPayload payload)
    {
      var country = _countriesService.GetCountryName(payload.Profile.CountryId)
        .Replace(" (" + payload.Profile.CountryId + ")", "");

      if (payload.ShippingType.Equals("Online"))
      {
        var contentWithShipping = new MultipartFormDataContent("----WebKitFormBoundary" + Guid.NewGuid())
        {
          {new StringContent(payload.ParsedRaffle.Wpfc7), "_wpcf7"},
          {new StringContent(payload.ParsedRaffle.Wpfc7Version), "_wpcf7_version"},
          {new StringContent(payload.ParsedRaffle.Locale), "_wpcf7_locale"},
          {new StringContent(payload.ParsedRaffle.UnitTag), "_wpcf7_unit_tag"},
          {new StringContent(payload.ParsedRaffle.Container), "_wpcf7_container_post"},
          {new StringContent("[]"), "_wpcf7cf_hidden_group_fields"},
          {new StringContent("[]"), "_wpcf7cf_hidden_groups"},
          {new StringContent($"[{payload.ParsedRaffle.Group}]"), "_wpcf7cf_visible_groups"},
          {new StringContent(payload.ParsedRaffle.Options), "_wpcf7cf_options"},
          {new StringContent("en"), "lang"},
          {new StringContent(payload.Profile.FirstName.Value), "your-firstname"},
          {new StringContent(payload.Profile.LastName.Value), "your-name"},
          {new StringContent(payload.Email), "your-email"},
          {new StringContent(payload.Instagram), "your-instagram"},
          {new StringContent(payload.Profile.PhoneNumber.Value), "your-tel"},
          {new StringContent("1"), "delivery[]"},
          {new StringContent(payload.Profile.AddressLine1.Value), "your-address"},
          {new StringContent(payload.Profile.PhoneNumber.Value), "your-postcode"},
          {new StringContent(payload.Profile.City.Value), "your-city"},
          {new StringContent(country), "your-country"},
          {new StringContent(payload.Size), "size"},
          {new StringContent("1"), "accept"},
          {new StringContent(payload.Captcha), "g-recaptcha-response"},
          {new StringContent(""), payload.ParsedRaffle.RaffleId}
        };

        return Task.FromResult(contentWithShipping);
      }

      var contentWithoutShipping = new MultipartFormDataContent("----WebKitFormBoundary" + Guid.NewGuid())
      {
        {new StringContent(payload.ParsedRaffle.Wpfc7), "_wpcf7"},
        {new StringContent(payload.ParsedRaffle.Wpfc7Version), "_wpcf7_version"},
        {new StringContent(payload.ParsedRaffle.Locale), "_wpcf7_locale"},
        {new StringContent(payload.ParsedRaffle.UnitTag), "_wpcf7_unit_tag"},
        {new StringContent(payload.ParsedRaffle.Container), "_wpcf7_container_post"},
        {
          new StringContent(@"[""your-address"",""your-postcode"",""your-city"",""your-country""]"),
          "_wpcf7cf_hidden_group_fields"
        },
        {new StringContent($"[{payload.ParsedRaffle.Group}]"), "_wpcf7cf_hidden_groups"},
        {new StringContent("[]"), "_wpcf7cf_visible_groups"},
        {new StringContent(payload.ParsedRaffle.Options), "_wpcf7cf_options"},
        {new StringContent("en"), "lang"},
        {new StringContent(payload.Profile.FirstName), "your-firstname"},
        {new StringContent(payload.Profile.LastName), "your-name"},
        {new StringContent(payload.Email), "your-email"},
        {new StringContent(payload.Instagram), "your-instagram"},
        {new StringContent(payload.Profile.PhoneNumber), "your-tel"},
        {new StringContent(""), "your-address"},
        {new StringContent(""), "your-postcode"},
        {new StringContent(""), "your-city"},
        {new StringContent(""), "your-country"},
        {new StringContent(payload.Size), "size"},
        {new StringContent("1"), "accept"},
        {new StringContent(payload.Captcha), "g-recaptcha-response"},
        {new StringContent(""), payload.ParsedRaffle.RaffleId}
      };

      return Task.FromResult(contentWithoutShipping);
    }
  }
}