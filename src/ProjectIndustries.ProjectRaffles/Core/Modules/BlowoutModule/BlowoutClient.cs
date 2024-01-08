using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.BlowoutModule
{
  public class BlowoutClient : ModuleHttpClientBase, IBlowoutClient
  {
    private readonly ICountriesService _countriesService;
    private readonly CookieContainer _cookieContainer = new CookieContainer();

    public BlowoutClient(ICountriesService countriesService)
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

    public Task<string> GenerateCsrfTokenAsync()
    {
      var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
      var stringChars = new char[30];
      var random = new Random();

      for (int i = 0; i < stringChars.Length; i++)
      {
        stringChars[i] = chars[random.Next(chars.Length)];
      }

      var finalString = new String(stringChars);

      return Task.FromResult(finalString);
    }

    public async Task<BlowoutParsedRaffle> ParseRaffleAsync(string raffleurl, CancellationToken ct)
    {
      var getRaffle = await HttpClient.GetAsync(raffleurl, ct);
      if (!getRaffle.IsSuccessStatusCode) await getRaffle.FailWithRootCauseAsync(ct: ct);

      var raffleContentBase = await getRaffle.Content.ReadAsStringAsync(ct);

      var doc1 = new HtmlDocument();
      doc1.LoadHtml(raffleContentBase);

      var formPost = doc1.DocumentNode.SelectSingleNode("//form[@id='support']").GetAttributeValue("action", "");
      var contestId = doc1.DocumentNode.SelectSingleNode("//input[@name='netiContestId']")
        .GetAttributeValue("value", "");

      return new BlowoutParsedRaffle(formPost, contestId);
    }
/*
    public async Task<string> GetCaptchaImage(CancellationToken ct)
    {
      var captchaUrl = "https://www.blowoutshop.de/widgets/Captcha/getCaptchaByName/captchaName/default";

      var blowoutBase = await HttpClient.GetAsync(captchaUrl, ct);
      var blowoutContentBase = await blowoutBase.ReadStringResultOrFailAsync("Can't access captcha", ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(blowoutContentBase);

      var captchaB64Image = doc.DocumentNode.SelectSingleNode("//img").GetAttributeValue("src", "");

      return captchaB64Image;
    } */

    public async Task<bool> SubmitAsync(BlowoutSubmitPayload payload, CancellationToken ct)
    {
      var csrfCookie = new Cookie("csrf_token-1", payload.CsrfToken) {Domain = "blowout.de"};
      _cookieContainer.Add(csrfCookie);

      var street = payload.Profile.AddressLine1.ToString().Replace(payload.HouseNumber, "");

      var country = _countriesService.GetCountryName(payload.Profile.CountryId)
        .Replace(" (" + payload.Profile.CountryId + ")", "");

      var content = new MultipartFormDataContent("----WebKitFormBoundary" + Guid.NewGuid())
      {
        {new StringContent("0"), "forceMail"},
        {new StringContent(payload.ParsedRaffle.ContestId), "netiContestId"},
        {new StringContent(payload.Profile.FirstName.Value), "vorname"},
        {new StringContent(payload.Profile.LastName.Value), "nachname"},
        {new StringContent(street), "Strasse"},
        {new StringContent(payload.HouseNumber), "Hausnummer"}, //todo: split up addressline1 and house number
        {new StringContent(payload.Profile.City.Value), "Stadt"},
        {new StringContent(payload.Profile.PostCode.Value), "PLZ"},
        {new StringContent(country), "Land"},
        {new StringContent(payload.Email), "email"},
        {new StringContent(payload.SizeValue), "Groesse"},
        {new StringContent(payload.InstagramHandle), "Insta"},
      //  {new StringContent(payload.PickUpLocation), "Pick"},
       // {new StringContent(payload.Captcha), "sCaptcha"},
        {new StringContent("honeypot"), "captchaName"},
        {new StringContent("1"), "privacy-checkbox"},
        {new StringContent("submit"), "Submit"},
        {new StringContent(payload.CsrfToken), "__csrf_token"}
      };


      var url = payload.ParsedRaffle.PostUrl;
      var signup = await HttpClient.PostAsync(url, content, ct);
      var signupHtml = await signup.Content.ReadAsStringAsync(ct);
      if (signupHtml.Contains("Bitte füllen Sie das Captcha-Feld korrek"))
      {
        await signup.FailWithRootCauseAsync("CAPTCHA error", ct);
      }

      if (!signupHtml.Contains("Thanks for joining")) await signup.FailWithRootCauseAsync(ct: ct);

      return signupHtml.Contains("Thanks for joining");
    }
  }
}