using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.ChmielnaModule
{
  public class ChmielnaClient : ModuleHttpClientBase, IChmielnaClient
  {
    private readonly ICountriesService _countriesService;
    private readonly IBirthdayProviderService _birthdayProvider;

    public ChmielnaClient(ICountriesService countriesService, IBirthdayProviderService birthdayProvider)
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

    public async Task<string> GetProductAsync(string raffleurl, CancellationToken ct)
    {
      var getPage = await HttpClient.GetAsync(raffleurl, ct);
      if(!getPage.IsSuccessStatusCode) await getPage.FailWithRootCauseAsync(ct: ct);
      
      var body = await getPage.Content.ReadAsStringAsync(ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;
      var title = node.Replace("Chmielna20.pl - ", "");

      return title;
    }

    public async Task<string> GetCountryCode(AddressFields profile, CancellationToken ct)
    {
      var apiUrl = $"http://www.geognos.com/api/en/countries/info/{profile.CountryId}.json";
      
      var getPage = await HttpClient.GetAsync(apiUrl, ct);
      var body = await getPage.ReadStringResultOrFailAsync("Error on submission", ct);

      var json = JObject.Parse(body);
      string countryId = (string) json["Results"]["TelPref"];


      return countryId;
    }

    public async Task<bool> SubmitAsync(AddressFields profile, string email, string raffleurl, string countryCode, string captcha, string size, CancellationToken ct)
    {
      var country = _countriesService.GetCountryName(profile.CountryId)
        .Replace(" (" + profile.CountryId + ")", "");

      var birthDay = await _birthdayProvider.GetDate();
      var birthMonth = await _birthdayProvider.GetMonth();
      var birthYear = await _birthdayProvider.GetYear();
      
      var dob = $"{birthYear}/{birthMonth}/{birthDay}";
      
      var content = new FormUrlEncodedContent( new Dictionary<string,string>
      {
        {"imie", profile.FirstName},
        {"nazwisko", profile.LastName},
        {"email", email},
        {"dial", countryCode},
        {"numer", profile.PhoneNumber},
        {"miasto", profile.City},
        {"kraj", country},
        {"data", dob},
        {"rozmiar", size},
        {"gresponse", captcha}
      });


      var endpoint = raffleurl + "/register.php";
      var resp1 = await HttpClient.PostAsync(endpoint, content, ct);
      var respHtml = await resp1.ReadStringResultOrFailAsync("Error on submission", ct);

      return respHtml.Contains(@"Thank You");
    }
  }
}