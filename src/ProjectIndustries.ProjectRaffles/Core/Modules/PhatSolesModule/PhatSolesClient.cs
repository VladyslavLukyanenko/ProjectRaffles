using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PhatSolesModule
{
  public class PhatSolesClient : ModuleHttpClientBase, IPhatSolesClient
  {
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

      return node;
    }

    public async Task<PhatSolesParsedRaffle> ParseRaffleAsync(string raffleUrl, CancellationToken ct)
    {
      var getRaffle = await HttpClient.GetAsync(raffleUrl, ct);
      var finalHtml = await getRaffle.ReadStringResultOrFailAsync("Can't access site", ct);
      
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
      
      var tel = raffleDoc.DocumentNode.SelectSingleNode("//input[@type='tel']")
        .GetAttributeValue("name", "");

      var regexPattern = @"class=""wpcf7-form-control-wrap menu-\d{1,4}"">";
      var regex = new Regex(regexPattern);
      var menuMatch = regex.Match(finalHtml).ToString();
      var menuName = menuMatch.Replace(@"class=""wpcf7-form-control-wrap ", "").Replace(@""">", "");

      return new PhatSolesParsedRaffle(wpfc7, wpfc7version, locale, unitTag, container, tel, menuName);
    }

    public async Task<bool> SubmitAsync(AddressFields profile, string email, PhatSolesParsedRaffle parsedRaffle, string captcha, string size, string instagram, CancellationToken ct)
    {
      var content = new MultipartFormDataContent("----WebKitFormBoundary" + Guid.NewGuid())
      {
        {new StringContent(parsedRaffle.Wpfc7), "_wpcf7"},
        {new StringContent(parsedRaffle.Wpfc7Version), "_wpcf7_version"},
        {new StringContent(parsedRaffle.Locale), "_wpcf7_locale"},
        {new StringContent(parsedRaffle.UnitTag), "_wpcf7_unit_tag"},
        {new StringContent(parsedRaffle.Container), "_wpcf7_container_post"},
        {new StringContent(""), "_wpcf7_posted_data_hash"},
        {new StringContent(captcha), "_wpcf7_recaptcha_response"},
        {new StringContent(profile.FirstName.Value), "First"},
        {new StringContent(profile.LastName.Value), "Last"},
        {new StringContent(email), "your-email"},
        {new StringContent(profile.PhoneNumber.Value), parsedRaffle.Tel},
        {new StringContent(instagram), "Instagram"},
        {new StringContent("Paypal"), parsedRaffle.Menu},
        {new StringContent(size), "thesize"}
      };

      var endpoint =
        $"https://phatsoles.com/wp-json/contact-form-7/v1/contact-forms/{parsedRaffle.Wpfc7}/feedback";
      var signup = await HttpClient.PostAsync(endpoint, content, ct);
      var signupHtml = await signup.ReadStringResultOrFailAsync("Error on submission", ct);
      
      if(signupHtml.Contains(@"""status"": ""spam""")) await signup.FailWithRootCauseAsync("Marked as spam", ct);

      return signupHtml.Contains(@"""status"": ""mail_sent""");
    }
  }
}