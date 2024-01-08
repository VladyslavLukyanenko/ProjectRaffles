using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Apm.Api;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.BrownsFashionModule
{
    public class BrownsFashionClient : ModuleHttpClientBase, IBrownsFashionClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.CookieContainer = _cookieContainer;
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("accept-language","en-US");
                httpClient.DefaultRequestHeaders.Add("Connection","keep-alive");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-dest","empty");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-mode","cors");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-site","same-origin");
                httpClient.DefaultRequestHeaders.Add("sec-gpc","1");
            };
        }

        public async Task<string> GetProductAsync(string raffleurl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleurl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access site");

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;

            return node;
        }

        public Task<string> GetRaffleNameAsync(string raffleurl, CancellationToken ct)
        {
            var regex = new Regex(@"https:\/\/www\.brownsfashion\.com\/.*\/raffles\/");
            var raffleMatch = regex.Match(raffleurl).ToString();
            var raffleName = raffleurl.Replace(raffleMatch, "");

            return Task.FromResult(raffleName);
        }

        public async Task<string> GetCurrencyCodeAsync(string countryCode, CancellationToken ct)
        {
            var restApi = $"https://restcountries.eu/rest/v2/alpha/{countryCode}";

            var getApi = await HttpClient.GetAsync(restApi, ct);
            var apiResponse = await getApi.ReadStringResultOrFailAsync("Can't access country API");

            dynamic apiJson = JObject.Parse(apiResponse);

            string currencyCode = (string) apiJson["currencies"][0]["code"];

            return currencyCode;
        }

        public async Task<string> LoginAsync(Account account, CancellationToken ct)
        {
            HttpClient.DefaultRequestHeaders.Add("origin","https://www.brownsfashion.com");
            HttpClient.DefaultRequestHeaders.Add("referer","https://www.brownsfashion.com/");
            HttpClient.DefaultRequestHeaders.Add("x-requested-with","XMLHttpRequest");
            var loginContentRoot = new
            {
                username = account.Email,
                password = account.Password,
                rememberMe = false
            };
            var loginUrl = "https://www.brownsfashion.com/dk/api/account/login";
            var loginjson = JsonConvert.SerializeObject(loginContentRoot);
            var loginContent = new StringContent(loginjson, Encoding.UTF8, "application/json");

            var postLogin = await HttpClient.PostAsync(loginUrl, loginContent, ct);
            if (!postLogin.IsSuccessStatusCode) await postLogin.FailWithRootCauseAsync("Error on login");

            return "";
        }

        public async Task<bool> SubmitAsync(AddressFields addressFields, Account account, string raffleName, string sizeValue, string currencyCode,
            CancellationToken ct)
        {
            // HttpClient.DefaultRequestHeaders.Add("Accept-Encoding","gzip, deflate, br");
            // HttpClient.DefaultRequestHeaders.Add("cache-control","private, must-revalidate");
            // HttpClient.DefaultRequestHeaders.Add("authority","www.brownsfashion.com");
            // HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("if-modified-since", "0");
            
            // HttpClient.DefaultRequestHeaders.Add("ff-country", addressFields.CountryId.Value);
            // HttpClient.DefaultRequestHeaders.Add("ff-currency", currencyCode);

            var registerPayload = new
            {
                formData = new
                {
                    firstName = addressFields.FirstName.Value,
                    lastName = addressFields.FirstName.Value,
                    email = account.Email,
                    dataUseConsent = true,
                    addressLine1 = addressFields.AddressLine1.Value,
                    city = addressFields.City.Value,
                    zipCode = addressFields.PostCode.Value,
                    phone = addressFields.PhoneNumber.Value,
                    countryCode = addressFields.CountryId.Value,
                    size = sizeValue,
                    rafflename = raffleName
                }
            };
            var json = JsonConvert.SerializeObject(registerPayload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.TryAddWithoutValidation("expires", "-1");
            
            var registerUrl = "https://www.brownsfashion.com/api/ffforms/api/v1/forms/raffle-entry/data?includeJsonSchema=true&resolveJsonSchemaPresets=true";
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri(registerUrl))
            {
                Content = content
            };

            message.Headers.Add("referer", "https://www.brownsfashion.com/");
            message.Headers.Add("x-requested-with", "XMLHttpRequest");
            message.Headers.Add("ff-country", addressFields.CountryId.Value);
            message.Headers.Add("ff-currency", currencyCode);
            message.Headers.Add("Accept-Encoding","gzip, deflate, br");
            message.Headers.Add("Accept","application/json, text/plain, */*");
            message.Headers.Add("Pragma","no-cache");
            message.Headers.Add("cache-control","private, must-revalidate");
            message.Headers.Add("authority","www.brownsfashion.com");
            message.Headers.TryAddWithoutValidation("if-modified-since", "0");
            var postContent = await HttpClient.SendAsync(message, ct);
            if(!postContent.IsSuccessStatusCode) await postContent.FailWithRootCauseAsync("Error on submission");

            return postContent.IsSuccessStatusCode;
        }
    }
}