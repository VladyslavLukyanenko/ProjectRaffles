using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.Basket4BallersModule
{
    public class Basket4BallersClient : ModuleHttpClientBase, IBasket4BallersClient
    {
        private readonly IMailchimpService _mailchimpService;
        private readonly CookieContainer _cookieContainer = new CookieContainer();

        public Basket4BallersClient(IMailchimpService mailchimpService)
        {
            _mailchimpService = mailchimpService;
        }
        
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.CookieContainer = _cookieContainer;
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("origin", "https://www.basket4ballers.com");
            };
        }
        
        public async Task<string> GetProductAsync(string raffleurl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleurl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access site", ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;

            return node;
        }

        public async Task<Basket4BallersParsedRaffle> ParseRaffleAsync(string raffleUrl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleUrl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access site", ct);
            
            var regex = new Regex(@"var countries = .*}};");
            var regexMatcher = regex.Match(body).ToString();
            var countryJsonMatch = regexMatcher.Replace("var countries = ", "").Replace(";", "");
            dynamic dynJson = JObject.Parse(countryJsonMatch);
            
            var countryDictionary = new Dictionary<string, string>();
            var stateDictionary = new Dictionary<string, string>();
            
            foreach (var item in dynJson)
            {
                string countryItem = Convert.ToString(item);
                string country = countryItem.Substring(countryItem.IndexOf("{"));
                dynamic countryJson = JObject.Parse(country);

                string countryId = countryJson.id_country;
                string countryIso = countryJson.iso_code;
                if (countryIso == "IT" || countryIso == "FR")
                {
                    foreach (var state in countryJson.states)
                    {
                        string stateId = state.id_state;
                        string stateIso = state.iso_code;

                        stateDictionary.Add(stateIso, stateId);
                    }
                }
                countryDictionary.Add(countryIso, countryId);
            }
            
            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var raffleId = doc.DocumentNode.SelectSingleNode("//input[@name='raffleRegistration[id_raffle]']").GetAttributeValue("value", "");
            var raffleToken = doc.DocumentNode.SelectSingleNode("//input[@name='raffleRegistration[token]']").GetAttributeValue("value", "");
            
            return new Basket4BallersParsedRaffle(raffleId, raffleToken, countryDictionary, stateDictionary);
        }

        public async Task<string> FindSizeAsync(string raffleUrl, string size, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleUrl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access site", ct);

            var sizeRegexPattern = @"<option value=""\d*"">" +size+ @" EUR<\/option>";
            var sizeRegex = new Regex(sizeRegexPattern);
            var matchSize = sizeRegex.Match(body).ToString();

            var sizeId = matchSize.Replace(@"<option value=""", "").Replace(size, "").Replace(@"""> EUR</option>", "");

            return sizeId;
        }

        public async Task<bool> SubmitAsync(AddressFields profile, Basket4BallersParsedRaffle parsedRaffle, string size, string captcha, string raffleurl, string useEmail, string instagram,
            CancellationToken ct)
        {
            HttpClient.DefaultRequestHeaders.Add("referer",raffleurl);
            //lookup profiles country ID (iso-code) against B4B's country ID's
            parsedRaffle.CountryDictionary.TryGetValue(profile.CountryId.Value.ToLower(), out string b4bCountryId);

            var stateId = "";
            //according to B4B apparently only italy and france have states
            if (profile.CountryId.Value == "IT" || profile.CountryId.Value == "FR")
            {
                parsedRaffle.StateDictionary.TryGetValue(profile.ProvinceId.Value.ToLower(), out stateId);
            }

            var addressline2 = "";
            if (profile.AddressLine2.Value != null)
            {
                addressline2 = profile.AddressLine2.Value;
            }
            
            var b4bJsonRoot = new
            {
                lastname = profile.LastName.Value,
                firstname = profile.FirstName.Value,
                email = useEmail,
                company = "",
                address1 = profile.AddressLine1.Value,
                address2 = addressline2,
                postcode = profile.PostCode.Value,
                city = profile.City.Value,
                id_country = b4bCountryId,
                id_state = stateId,
                phone = profile.PhoneNumber.Value,
                id_facebook = profile.FirstName.Value + " " + profile.LastName.Value,
                id_instagram = instagram,
                id_attribute_choice = size,
                newsletter = "1",
                age = "1",
                id_raffle = parsedRaffle.RaffleId,
                token = parsedRaffle.RaffleToken
            };
            var jsonContent = JsonConvert.SerializeObject(b4bJsonRoot);
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"ajax","true"},
                {"token", parsedRaffle.RaffleToken},
                {"action","submitRaffleRegistration"},
                {"raffleRegistration", jsonContent},
                {"g-recaptcha-response", captcha}
            });

            var unixTime = await _mailchimpService.GetUnixTime();
            var endpoint = $"https://www.basket4ballers.com/modules/b4b_raffle/ajax/?rand={unixTime}";
            
            var postRegistration = await HttpClient.PostAsync(endpoint, content, ct);
            var postBody = await postRegistration.ReadStringResultOrFailAsync("Error on submission", ct);
            
            if(!postBody.Contains(@"{""errors"":[]}")) await postRegistration.FailWithRootCauseAsync("Error on submission", ct);

            return postBody.Contains(@"{""errors"":[]}");
        }
    }
}