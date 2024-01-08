using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WoeiModule
{
    public class WoeiClient : ModuleHttpClientBase, IWoeiClient
    {
        private readonly ICountriesService _countriesService;
        private readonly IBirthdayProviderService _birthdayProvider;
        private readonly CookieContainer _cookieContainer = new CookieContainer();

        public WoeiClient(ICountriesService countriesService, IBirthdayProviderService birthdayProvider)
        {
            _countriesService = countriesService;
            _birthdayProvider = birthdayProvider;
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
                httpClient.DefaultRequestHeaders.Add("Origin","https://www.woei-webshop.nl");
            };
        }

        public async Task LoginAsync(Account account, CancellationToken ct)
        {
            var getLoginSite = await HttpClient.GetAsync("https://www.woei-webshop.nl/en/account/login/", ct);
            var loginSiteResponse = await getLoginSite.ReadStringResultOrFailAsync("Cannot access loginsite", ct);
            
            var html = new HtmlDocument();
            html.LoadHtml(loginSiteResponse);

            HttpClient.DefaultRequestHeaders.Add("Referer", "https://www.woei-webshop.nl/en/account/login/");
            var key = html.DocumentNode.SelectSingleNode("//input[@name='key']").GetAttributeValue("value", "");
            var postLoginDetailsContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"key", key},
                {"email", account.Email},
                {"password", account.Password}
            });

            var postLogin = await HttpClient.PostAsync("https://www.woei-webshop.nl/en/account/loginPost/",
                postLoginDetailsContent, ct);
            if (!postLogin.IsSuccessStatusCode) await postLogin.FailWithRootCauseAsync("Error on login", ct);
            HttpClient.DefaultRequestHeaders.Remove("Referer");
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

        public async Task<WoeiParsed> ParseProductAsync(string raffleUrl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleUrl, ct); 
            var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var product = doc.DocumentNode.SelectSingleNode("//input[@name='product']").GetAttributeValue("value", "").Replace("&quot;",@"""");

            var image = doc.DocumentNode.SelectSingleNode("//input[@name='productImage']").GetAttributeValue("value", "");

            var id = doc.DocumentNode.SelectSingleNode("//input[@name='productId']").GetAttributeValue("value", "");

            return new WoeiParsed(product, image, id);
        }

        public async Task<bool> SubmitAsync(AddressFields profile, WoeiParsed parsedRaffle, Account account, string instagram, string size, string gender, CancellationToken ct)
        {
            var age = await _birthdayProvider.GenerateAge();
      
            var country = _countriesService.GetCountryName(profile.CountryId)
                .Replace(" (" + profile.CountryId + ")", "");
      
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"product", parsedRaffle.Product},
                {"productImage", parsedRaffle.ProductImage},
                {"productId", parsedRaffle.ProductId},
                {"dontfill", ""},
                {"firstname", profile.FirstName.Value},
                {"lastname", profile.LastName.Value},
                {"age", $"{age}"},
                {"gender", gender},
                {"shoeSize", $"Size : {size}"},
                {"address", profile.AddressLine1.Value},
                {"zip", profile.PostCode.Value},
                {"country", country},
                {"email", account.Email},
                {"phone", profile.PhoneNumber.Value},
                {"instagram", instagram}
            });
            
            HttpClient.DefaultRequestHeaders.Add("referer", "https://www.woei-webshop.nl/");
            
            var endpoint = "https://apps.shopmonkey.nl/panthers/raffle/raffle.php";
            var signup = await HttpClient.PostAsync(endpoint, content, ct);
            var response = await signup.ReadStringResultOrFailAsync("Error on submission", ct);
            
            return response.Contains("success");
        }
    }
}