using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.VoostoreBerlinModule
{
  public class VoostoreBerlinClient : ModuleHttpClientBase, IVoostoreBerlinClient
  {
    private readonly ICountriesService _countriesService;
    private readonly CookieContainer _cookieContainer = new CookieContainer();
    private readonly IPhoneCodeService _phoneCodeService;

    public VoostoreBerlinClient(ICountriesService countriesService, IPhoneCodeService phoneCodeService)
    {
      _countriesService = countriesService;
      _phoneCodeService = phoneCodeService;
    }

    protected override void ConfigureHttpClient(HttpClientOptions options)
    {
      options.AllowAutoRedirect = false;
      options.CookieContainer = _cookieContainer;
      options.PostConfigure = httpClient =>
      {
        httpClient.DefaultRequestHeaders.Add("User-Agent",
          "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        httpClient.DefaultRequestHeaders.Add("dnt", "1");
      };
    }

    public async Task<VoostoreBerlinRaffleTags> GetRaffleTagsAsync(string raffleurl, CancellationToken ct)
    {
      var getRaffle = await HttpClient.GetAsync(raffleurl, ct);
      var finalHtml = await getRaffle.ReadStringResultOrFailAsync("Can't access page", ct);
      
      var raffleDoc = new HtmlDocument();
      raffleDoc.LoadHtml(finalHtml);

      var tokenHTML = raffleDoc.DocumentNode.SelectSingleNode("//input[@name='token']");
      var token = tokenHTML.GetAttributeValue("value", "");
      var pageidHTML = raffleDoc.DocumentNode.SelectSingleNode("//input[@name='page_id']");
      var pageid = pageidHTML.GetAttributeValue("value", "");

      return new VoostoreBerlinRaffleTags(token, pageid);
    }

    public async Task<bool> SubmitAsync(VoostoreBerlinSubmitPayload payload, CancellationToken ct)
    {
      var countryCode = await _phoneCodeService.GetPhoneCodeAsync(payload.Profile.CountryId, ct);
      
      var country = _countriesService.GetCountryName(payload.Profile.CountryId)
        .Replace(" (" + payload.Profile.CountryId + ")", "");
      
      var getRaffle = await HttpClient.GetAsync(payload.RaffleUrl, ct);
      var raffleHtml = await getRaffle.ReadStringResultOrFailAsync("Can't access site", ct);
      
      //match size
      var sizeRegexPattern = @"size\('\d*'\)""><a class=""a_top_hypers""  >  "+ payload.SizeValue;
      var sizeRegex = new Regex(sizeRegexPattern);
      var findSizeMatch = sizeRegex.Match(raffleHtml).ToString();
      
      var removeSizeRegexPattern = @"'\)""><a class=""a_top_hypers""  >  " + payload.SizeValue;
      var removeSizeRegex = new Regex(removeSizeRegexPattern);
      var removeSizeMatch = removeSizeRegex.Match(findSizeMatch).ToString();
      var size = findSizeMatch.Replace(removeSizeMatch, "").Replace(@"size('", "");
      
      
      var street = payload.Profile.AddressLine1.ToString().Replace(payload.HouseNumber, "");

      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"token", payload.RaffleTags.Token},
        {"page_id", payload.RaffleTags.PageId},
        {"shoes_size", size},
        {"action", "send_request"},
        {"fax", ""},
        {"name", payload.Profile.FirstName},
        {"lastname", payload.Profile.LastName},
        {"email", payload.Email},
        {"contact_number", "00" + countryCode + payload.Profile.PhoneNumber}, //format like "00{countrycode}number"
        {"streetname", street}, 
        {"housenumber", payload.HouseNumber},
        {"postalcode", payload.Profile.PostCode},
        {"city", payload.Profile.City},
        {"country", country},
        {"countryhidden", ""},
        {"g-recaptcha-response", payload.Captcha}
      });

      var postUrl = "https://raffle.vooberlin.com/ajax.php";
      var response = await HttpClient.PostAsync(postUrl, content, ct);
      if(!response.IsSuccessStatusCode) await response.FailWithRootCauseAsync("Can't submit entry", ct);

      return response.IsSuccessStatusCode;
    }
  }
}