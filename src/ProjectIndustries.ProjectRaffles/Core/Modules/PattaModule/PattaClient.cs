using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PattaModule
{
    public class PattaClient : ModuleHttpClientBase, IPattaClient
    {
        private readonly IBirthdayProviderService _birthdayProvider;
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        

        public PattaClient(IBirthdayProviderService birthdayProvider)
        {
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
                httpClient.DefaultRequestHeaders.Add("origin","https://www.patta.nl");
            };
        }

        public async Task<string> GetProductAsync(string raffleUrl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleUrl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;

            return node;
        }

        public async Task LoginAsync(Account account, CancellationToken ct)
        {
            HttpClient.DefaultRequestHeaders.Add("referer", "https://www.patta.nl/account/login");
            
            var postData = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"form_type", "customer_login"},
                {"utf8","✓"},
                {"customer[email]", account.Email},
                {"customer[password]", account.Password}
            });

            var postLogin = await HttpClient.PostAsync("https://www.patta.nl/account/login", postData, ct);
            var response = await postLogin.ReadStringResultOrFailAsync("Cannot login", ct);
            
            if(!response.Contains("Account")) throw new RaffleFailedException("Error on login", "Error on login with email: " + account.Email);
            HttpClient.DefaultRequestHeaders.Remove("referer");
        }

        public async Task<PattaParsed> ParseRaffleAsync(string raffleUrl, string sizeVariant, CancellationToken ct)
        {
            var getRaffle = await HttpClient.GetAsync(raffleUrl, ct);
            var response = await getRaffle.ReadStringResultOrFailAsync("Cannot access raffle page", ct);
            
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response);

            var productId = htmlDocument.DocumentNode.SelectSingleNode("//input[@id='product-id']").GetAttributeValue("value", "");
            var raffleId = htmlDocument.DocumentNode.SelectSingleNode("//input[@id='article-id']").GetAttributeValue("value", "");
            var raffleName = htmlDocument.DocumentNode.SelectSingleNode("//input[@id='product_handle']").GetAttributeValue("value", "");
            
            var sizeVariantId = htmlDocument.DocumentNode.SelectSingleNode($"//input[@data-sku='{sizeVariant}']").GetAttributeValue("data-variant-id", "");
            
            
            return new PattaParsed(productId, sizeVariantId, raffleName, raffleId);
        }

        public async Task<bool> SubmitAsync(AddressFields addressFields, Account account, PattaParsed parsed, string sizeSku, string raffleUrl, string instagramAccount, CancellationToken ct)
        {
            var getRaffle = await HttpClient.GetAsync(raffleUrl, ct);
            var response = await getRaffle.ReadStringResultOrFailAsync("Cannot access raffle page", ct);
            
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response);
            
            var customerIdString = htmlDocument.DocumentNode.SelectSingleNode("//input[@id='customer-id']").GetAttributeValue("value", "");

            var dob = $"{await _birthdayProvider.GetDate()}/{await _birthdayProvider.GetMonth()}/{await _birthdayProvider.GetYear()}";
            var contentRoot = new
            {
                bday = dob,
                city = addressFields.City.Value,
                country = "Netherlands",
                customerId = customerIdString,
                email = account.Email,
                firstName = addressFields.FirstName.Value,
                instagram = instagramAccount,
                lastName = addressFields.LastName.Value,
                productID = parsed.ProductId,
                productSizeSKU = sizeSku,
                productSlug = parsed.RaffleName,
                productVariantId = parsed.ProductVariantId,
                raffleId = parsed.RaffleId,
                raffleName = parsed.RaffleName,
                streetAddress = addressFields.AddressLine1.Value,
                zipCode = addressFields.PostCode.Value
            };
            var json = JsonConvert.SerializeObject(contentRoot);
            var content = new StringContent(json, Encoding.UTF8, "text/plain;charset=UTF-8");

            var endpoint = "https://patta-raffle.vercel.app/api/postRaffleEntry/";

            var post = await HttpClient.PostAsync(endpoint, content, ct);
            var postContent = await post.ReadStringResultOrFailAsync("Error on submission", ct);

            return postContent.Contains("Added to raffle form");
        }
    }
}