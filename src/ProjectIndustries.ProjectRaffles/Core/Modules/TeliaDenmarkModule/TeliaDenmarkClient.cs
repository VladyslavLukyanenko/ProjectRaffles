using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TeliaDenmarkModule
{
    public class TeliaDenmarkClient : ModuleHttpClientBase, ITeliaDenmarkClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();

        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.CookieContainer = _cookieContainer;
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.193 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-User","?1");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site","same-origin");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode","navigate");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest","document");
                httpClient.DefaultRequestHeaders.Add("Accept-Language","en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
                httpClient.DefaultRequestHeaders.Add("Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
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

        public async Task<bool> SubmitAsync(AddressFields addressFields, string email, string raffleUrl, string captcha, CancellationToken ct)
        {
            //get cookies
            var getPage = await HttpClient.GetAsync(raffleUrl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

            //get request token
            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var requestToken = doc.DocumentNode.SelectSingleNode("//input[@name='__RequestVerificationToken']").GetAttributeValue("value", "");

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"__RequestVerificationToken", requestToken},
                {"Name", addressFields.FullName},
                {"Email", email},
                {"Phonenumber", addressFields.PhoneNumber},
                {"Consent","true"},
                {"g-recaptcha-response", captcha}
            });

            HttpClient.DefaultRequestHeaders.Add("Referer",raffleUrl);
            HttpClient.DefaultRequestHeaders.Add("Origin","https://www.telia.dk");
            var postContent = await HttpClient.PostAsync(raffleUrl, content, ct);
            var postBody = await postContent.ReadStringResultOrFailAsync("Error on submission", ct);

            return postBody.Contains("Tak for din tilmelding");
        }
    }
}