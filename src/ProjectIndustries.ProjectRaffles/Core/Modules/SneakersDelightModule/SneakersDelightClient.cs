using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SneakersDelightModule
{
    public class SneakersDelightClient : ModuleHttpClientBase, ISneakersDelightClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private readonly IMailchimpService _mailchimpService;
        private readonly ICaptchaSolveService _captchaSolver;

        public SneakersDelightClient(IMailchimpService mailchimpService, ICaptchaSolveService captchaSolver)
        {
            _mailchimpService = mailchimpService;
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

        public async Task<string> GetProductAsync(string raffleurl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleurl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access raffle", ct);
            
            var raffleRegex = new Regex(@"""raffle"": {.*}");
            var raffleRegexMatch = raffleRegex.Match(body).ToString().Replace(@"""raffle"": ", "");
            
            dynamic raffleJson = JObject.Parse(raffleRegexMatch);
            string raffleName = raffleJson.name;
            
            return raffleName;
        }

        public async Task<SneakersDelightParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct)
        {
            var getSite = await HttpClient.GetAsync(raffleUrl, ct);
            var body = await getSite.ReadStringResultOrFailAsync("Can't access raffle", ct);
            
            var raffleRegex = new Regex(@"""raffle"": {.*}");
            var raffleRegexMatch = raffleRegex.Match(body).ToString().Replace(@"""raffle"": ", "");
            
            dynamic raffleJson = JObject.Parse(raffleRegexMatch);
            string raffleId = raffleJson.id;
            
            var sizeRegex = new Regex(@"""product"":.*}]}");
            var sizeRegexMatch = sizeRegex.Match(body).ToString().Replace(@"""product"":", "");;

            dynamic prepareJson = JObject.Parse(sizeRegexMatch);
            
            var sizeDictionary = new Dictionary<string,string>();
            dynamic sizeObjects = (JArray) prepareJson["configurable_options"][0]["options"];
            foreach (var sizeObject in sizeObjects)
            {
                string size = sizeObject.label;
                string sizeId = sizeObject.product_id;
                
                sizeDictionary.Add(size, sizeId);
            }
            
            return new SneakersDelightParsed(raffleId, sizeDictionary);
        }

        public async Task<string> LoginAsync(Account account,CancellationToken ct)
        {
            var captcha = await _captchaSolver.SolveReCaptchaV3Async("6LeVmPkUAAAAAIhCpWvdie7d7XJzW2bnpjWOO4nC",
                "https://sneakersdelight.store/customer/account/login/", "verify",0.3, ct);

            var getSite = await HttpClient.GetAsync("https://sneakersdelight.store/customer/account/login/", ct);
            var body = await getSite.ReadStringResultOrFailAsync("Can't get login page", ct);
            
            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var referer = doc.DocumentNode.SelectSingleNode("//input[@name='referer']").GetAttributeValue("value", "");
            var formKey = doc.DocumentNode.SelectSingleNode("//input[@name='form_key']").GetAttributeValue("value", "");

            var loginContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"form_key", formKey},
                {"referer", referer},
                {"login[username]", account.Email},
                {"login[password]", account.Password},
                {"minty_invisible_token", captcha}
            });

            var loginEndpoint = "https://sneakersdelight.store/customer/account/loginPost/";
            var loginPost = await HttpClient.PostAsync(loginEndpoint, loginContent, ct);
            if (!loginPost.IsSuccessStatusCode) await loginPost.FailWithRootCauseAsync("Can't login", ct);
            
            var formKeyCookie = new Cookie("form_key", formKey) {Domain = "sneakersdelight.store"};
            _cookieContainer.Add(formKeyCookie); //needed for all future requests
            
            return "";
        }

        public async Task<SneakersDelightUserData> GetAccountInformationAsync(AddressFields addressFields, CancellationToken ct)
        {
            var time = await _mailchimpService.GetUnixTime();

            var getUserDataEndpoint = $"https://sneakersdelight.store/rest/V1/customers/me?_={time}";
            
            HttpClient.DefaultRequestHeaders.Add("x-requested-with","XMLHttpRequest"); //without this, all requests will get 401
            var getUserData = await HttpClient.GetAsync(getUserDataEndpoint, ct);
            var userData = await getUserData.ReadStringResultOrFailAsync("Can't get user data", ct);
            
            var deserializeUserData = JsonConvert.DeserializeObject<SneakersDelightDeserializeUserData>(userData);

            if (deserializeUserData.addresses.Count == 0)
            {
                var addressList = new List<SneakersDelightAddressInformation>();
                var addressLine2 = "";
                if (addressFields.AddressLine2 != null)
                {
                    addressLine2 = addressFields.AddressLine2;
                }
                
                var streetList = new List<string> {addressFields.AddressLine1, addressLine2};
                var addresses = new SneakersDelightAddressInformation()
                {
                    firstname = deserializeUserData.firstname,
                    city = addressFields.City,
                    country_id = addressFields.CountryId,
                    customer_id = deserializeUserData.id,
                    lastname = deserializeUserData.lastname,
                    postcode = addressFields.PostCode,
                    region = "",
                    region_id = null,
                    street = streetList,
                    telephone = addressFields.PhoneNumber,
                    vat_id = ""
                };
                addressList.Add(addresses);

                var changeAddressInfo = new SneakersDelightAccountInformation()
                {
                    customer = new SneakersDelightCustomerInformation()
                    {
                        addresses = addressList,
                        id = deserializeUserData.id,
                        group_id = deserializeUserData.group_id,
                        created_at = deserializeUserData.created_at,
                        updated_at = deserializeUserData.updated_at,
                        created_in = deserializeUserData.created_in,
                        disable_auto_group_change = deserializeUserData.disable_auto_group_change,
                        dob = deserializeUserData.dob,
                        email = deserializeUserData.email,
                        firstname = deserializeUserData.firstname,
                        lastname = deserializeUserData.lastname,
                        gender = deserializeUserData.gender,
                        store_id = deserializeUserData.store_id,
                        website_id = deserializeUserData.website_id,
                        extension_attributes = new SneakersDelightExtensionAttributes
                        {
                            is_subscribed = deserializeUserData.extension_attributes.is_subscribed
                        }
                    }
                };
                var json = JsonConvert.SerializeObject(changeAddressInfo); 
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var endpoint = "https://sneakersdelight.store/rest/V1/customers/me";

                var putRequest = await HttpClient.PutAsync(endpoint, content, ct);
                if (!putRequest.IsSuccessStatusCode)
                    await putRequest.FailWithRootCauseAsync("Can't submit new address info", ct);
                
                time = await _mailchimpService.GetUnixTime();
                getUserDataEndpoint = $"https://sneakersdelight.store/rest/V1/customers/me?_={time}";
                getUserData = await HttpClient.GetAsync(getUserDataEndpoint, ct);
                userData = await getUserData.ReadStringResultOrFailAsync("Can't get user data 2", ct);
                
                deserializeUserData = JsonConvert.DeserializeObject<SneakersDelightDeserializeUserData>(userData);
            }

            var streets = new List<object>();
            foreach (var addressObject in deserializeUserData.addresses)
            {
                streets.Add(addressObject);
            }

            return new SneakersDelightUserData(deserializeUserData, streets);
        }

        public async Task<bool> SubmitEntryAsync(SneakersDelightParsed parsed, string size, string raffleurl, SneakersDelightUserData accountInfo,
            CancellationToken ct)
        {
            var addressObject = accountInfo.UserData.addresses[0];
                
                var jsonContent = new SneakersDelightRaffleJson()
            {
                raffle_id = "7",
                product_id = size,
                address = addressObject
            };
            var json = JsonConvert.SerializeObject(jsonContent);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient.DefaultRequestHeaders.Add("referer",raffleurl);
            var endpoint = "https://sneakersdelight.store/rest/V1/minty-raffle/enter-raffle";
            var postContent = await HttpClient.PostAsync(endpoint, content, ct);
            if (!postContent.IsSuccessStatusCode) await postContent.FailWithRootCauseAsync("Can't submit entry", ct);

            return postContent.IsSuccessStatusCode;
        }
    }
}