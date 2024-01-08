using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.FenomModule
{
    public class FenomClient : ModuleHttpClientBase, IFenomClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        
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

        public async Task<string> GetProductNameAsync(string raffleurl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleurl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access raffle", ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;

            return node;
        }

        public async Task<FenomSizeIdentifiers> GetSizeIdentifiersAsync(string raffleurl, string size,
            CancellationToken ct)
        {
            var raffleGet = await HttpClient.GetAsync(raffleurl, ct);
            var raffleBody = await raffleGet.ReadStringResultOrFailAsync("Can't access site", ct);

            var sizeRegexPattern = @"<label for=""radio_group_\d{1,3}_attribute_\d{1,3}"" class=""units_container"">\n<span class=""size_EU"">\n"+size;
            var sizeRegex = new Regex(sizeRegexPattern);
            var sizeMatch = sizeRegex.Match(raffleBody).ToString();
            
            var doc = new HtmlDocument();
            doc.LoadHtml(raffleBody);

            var productId = doc.DocumentNode.SelectSingleNode("//input[@name='id_product']")
                .GetAttributeValue("value", "");
            
            var groupId = new Regex(@"group_\d{1,3}").Match(sizeMatch).ToString().Replace("group_","");
            var attributeId = new Regex(@"attribute_\d{1,3}").Match(sizeMatch).ToString().Replace("attribute_", "");
            
            return new FenomSizeIdentifiers(productId, groupId, attributeId);
        }

        public async Task<string> LoginAsync(Account account, CancellationToken ct)
        {
            var loginUrl = "https://www.fenom.com/en/authentication";
            
            //get prestaShop cookies before login
            var prestaShop = await HttpClient.GetAsync(loginUrl, ct);
            if(!prestaShop.IsSuccessStatusCode) await prestaShop.FailWithRootCauseAsync("Can't access login page", ct);
            
            var loginContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"back",""},
                {"email", account.Email},
                {"password", account.Password},
                {"submitLogin", "1"}
            });
            
            HttpClient.DefaultRequestHeaders.Add("origin","https://www.fenom.com");
            HttpClient.DefaultRequestHeaders.Add("referer", loginUrl);

            var postLogin = await HttpClient.PostAsync(loginUrl, loginContent, ct);
            if (!postLogin.IsSuccessStatusCode) await postLogin.FailWithRootCauseAsync("Can't login", ct);
            
            HttpClient.DefaultRequestHeaders.Remove("referer");

            return "";
        }

        public async Task<bool> SubmitEntryAsync(Account account, FenomSizeIdentifiers sizeIdentifiers, string raffleurl, string captcha,
            CancellationToken ct)
        {
            var raffleContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"recaptcha", captcha},
                {"id_product", sizeIdentifiers.ProductId},
                {"id_attribute_group", sizeIdentifiers.GroupId},
                {"attribute_group_value", sizeIdentifiers.AttributionId}
            });

            HttpClient.DefaultRequestHeaders.Add("x-requested-with","XMLHttpRequest");
            HttpClient.DefaultRequestHeaders.Add("referer",raffleurl);
            var postRaffleData =
                await HttpClient.PostAsync("https://www.fenom.com/fr/launch-ajax?ajax=1", raffleContent, ct);
            
            var postRaffleBody =
                await postRaffleData.ReadStringResultOrFailAsync($"Error on submission for email: {account.Email}", ct);

            return postRaffleBody.Contains("1");
        }
    }
}