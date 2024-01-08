using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.WoeiAccountGenerator
{
    public class WoeiAccountGeneratorClient : IWoeiAccountGeneratorClient
    {
      private readonly CookieContainer _cookieContainer = new CookieContainer();
      private readonly IHttpClientBuilder _builder;
      private HttpClient _httpClient;
      private readonly IPhoneCodeService _phoneCodeService;
    
      public WoeiAccountGeneratorClient(IHttpClientBuilder builder, IPhoneCodeService phoneCodeService)
      {
          _builder = builder;
          _phoneCodeService = phoneCodeService;
      }
      
      public void GenerateHttpClient()
      {
          _httpClient = _builder.WithConfiguration(ConfigureHttpClient)
              .Build();
      }

      private void ConfigureHttpClient(HttpClientOptions options)
      {
        options.CookieContainer = _cookieContainer;
        options.AllowAutoRedirect = true;
        options.PostConfigure = httpClient =>
        {
          httpClient.DefaultRequestHeaders.Add("User-Agent",
           "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
          httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
          httpClient.DefaultRequestHeaders.Add("dnt", "1");
        };
      }

      public async Task<WoeiAccountGeneratorParsed> GetCountriesAsync(CancellationToken ct)
      {
        var url = "https://www.woei-webshop.nl/en/account/register/";
        var getSite = await _httpClient.GetAsync(url, ct);
        var body = await getSite.ReadStringResultOrFailAsync("Can't access register page");

       var countryRegex = new Regex("var gui_countries.*");
       var matchCountry = countryRegex.Match(body).ToString();

       var countryObjectJson = matchCountry.Replace("var gui_countries = ", "").Replace(";", "");

       dynamic parsedJson = JObject.Parse(countryObjectJson);

       var countryDictionary = new Dictionary<string, string>();
       var hasRegionsDictionary = new Dictionary<string, bool>();
       var countryType = new Dictionary<string, string>();
       foreach (var countryObject in parsedJson)
        {
          string countryItem = Convert.ToString(countryObject);
          string country = countryItem.Substring(countryItem.IndexOf("{", StringComparison.Ordinal));
          dynamic countryJson = JObject.Parse(country);

          string countryId = countryJson.id;
          string countryCode = countryJson.code;
          countryDictionary.Add(countryCode, countryId);

          bool countryRegions = countryJson.has_regions;

           string type = countryJson.type;

          hasRegionsDictionary.Add(countryCode, countryRegions);
          countryType.Add(countryCode, type);
        }

        return new WoeiAccountGeneratorParsed(countryDictionary, hasRegionsDictionary, countryType);
      }

     public async Task<string> GetCountryRegionIdAsync(string country, string province, CancellationToken ct)
     {
         var regionUrl = $"https://www.woei-webshop.nl/en/services/regions/?country={country}";
         var getRegionPage = await _httpClient.GetAsync(regionUrl, ct);
         var regionPageBody = await getRegionPage.ReadStringResultOrFailAsync("Failed on getting regions");
         dynamic regionJsonObject = JObject.Parse(regionPageBody);

         var regionDictionary = new Dictionary<string, string>();
         foreach (var regionObject in regionJsonObject.regions)
         {
             string regionItem = Convert.ToString(regionObject);
             string region = regionItem.Substring(regionItem.IndexOf("{", StringComparison.Ordinal));
             dynamic regionJson = JObject.Parse(region);

             string regionId = regionJson.id;
             string regionCode = regionJson.code;
             regionDictionary.Add(regionCode, regionId);
         }
         
         regionDictionary.TryGetValue(province, out string lookedUpRegion);

         return lookedUpRegion; 
     }

     public async Task<string> GetPhoneCodeAsync(string country, CancellationToken ct)
     {
         var countryId = await _phoneCodeService.GetPhoneCodeAsync(country, ct);

         var countryUpper = country.ToUpper();
         var phoneCode = $"{countryUpper}|{countryId}";
         
         return phoneCode;
     }

     public async Task<string> SubmitAccountAsync(AddressFields addressFields, string email, string type,
       string houseNumber, string country, string provinceId, string phonecode, CancellationToken ct)
      { 
          var url = "https://www.woei-webshop.nl/en/account/register/";
          var getSite = await _httpClient.GetAsync(url, ct);
          var body = await getSite.ReadStringResultOrFailAsync("Can't access register page");

          var doc = new HtmlDocument();
          doc.LoadHtml(body);

          string streetString = addressFields.AddressLine1;
          var street = streetString.Replace(houseNumber, "");

          var key = "";
          try
          {
             key = doc.DocumentNode.SelectSingleNode("//input[@name='key']").GetAttributeValue("value", "");
          }
          catch (NullReferenceException)
          {
              await getSite.FailWithRootCauseAsync("Can't parse");
          }

          var content = new FormUrlEncodedContent(new Dictionary<string, string>
          {//39
              {"key", key},
              {"gender", ""},
              {"firstname", addressFields.FirstName},
              {"middlename", ""},
              {"lastname", addressFields.LastName},
              {"email", email},
              {"type", "private"},
              {"phone_number_code", phonecode},
              {"phone", addressFields.PhoneNumber},
              {"mobile", ""},
              {"format", type},
              {"streetname", street},
              {"streetname2", addressFields.AddressLine2},
              {"number", houseNumber},
              {"house_extension", ""},
              {"zipcode", addressFields.PostCode},
              {"city", addressFields.City},
              {"region_id", provinceId},
              {"region", ""},
              {"country", country},
              {"same_address", "1"},
              {"shipping_attention", ""},
              {"shipping_company", ""},
              {"shipping_format", type},
              {"shipping_streetname", ""},
              {"shipping_streetname2", ""},
              {"shipping_number", ""},
              {"shipping_house_extension", ""},
              {"shipping_zipcode", ""},
              {"shipping_city", ""},
              {"shipping_region_id", provinceId},
              {"shipping_region", ""},
              {"shipping_country", country},
              {"company", ""},
              {"cocnumber", ""},
              {"vatnumber", ""},
              {"password", "ProjectRaffles!1)3!"},
              {"password2", "ProjectRaffles!1)3!"},
              {"terms", "1"}
          });

          _httpClient.DefaultRequestHeaders.Add("origin","https://www.woei-webshop.nl");
          _httpClient.DefaultRequestHeaders.Add("referer","https://www.woei-webshop.nl/en/account/register/");
          var endpoint = "https://www.woei-webshop.nl/en/account/registerPost/";
          var postAccount = await _httpClient.PostAsync(endpoint, content, ct); 
          if (!postAccount.IsSuccessStatusCode) await postAccount.FailWithRootCauseAsync("Error on submission 1");
          return key;
      }

      public async Task<bool> SubmitCaptchaAsync(string authtoken, string captcha, CancellationToken ct)
      {
          var content = new FormUrlEncodedContent(new Dictionary<string, string>
          {
             {"key", authtoken},
             {"g-recaptcha-response", captcha}
          });
          
          _httpClient.DefaultRequestHeaders.Remove("referer");
          _httpClient.DefaultRequestHeaders.Add("referer","https://www.woei-webshop.nl/en/services/challenge/");
          var endpoint = "https://www.woei-webshop.nl/en/account/registerPost/";
          var post = await _httpClient.PostAsync(endpoint, content, ct);
          var body = await post.ReadStringResultOrFailAsync("Error on submission 2");

          return body.Contains("Edit account information"); 
      } 
    }
}