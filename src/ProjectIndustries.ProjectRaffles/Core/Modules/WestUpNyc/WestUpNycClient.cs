using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WestUpNyc
{
    public class WestUpNycClient : ModuleHttpClientBase, IWestUpNycClient
    {
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("Accept-Language",
                    "en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
                httpClient.DefaultRequestHeaders.Add("sec-gpc", "1");
            };
        }
        
        public async Task<string> GetProductAsync(string raffleUrl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleUrl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access site", ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;

            return node;
        }

        public async Task<WestUpNycParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct)
        {
            var requestUrl = await HttpClient.GetAsync(raffleUrl, ct);
            var siteContent = await requestUrl.ReadStringResultOrFailAsync("Can't access page", ct);
            
            var baseDoc = new HtmlDocument();
            baseDoc.LoadHtml(siteContent);

            var formUrl = baseDoc.DocumentNode.SelectSingleNode("//iframe[@id='klaviyo_subscribe_page']")
                .GetAttributeValue("src", "");

            var getForm = await HttpClient.GetAsync(formUrl, ct);
            var formContent = await getForm.ReadStringResultOrFailAsync("Can't access form", ct);
            
            var sizeField = new Regex(@"name="".*Size"" ").Match(formContent).ToString().Replace(@"name=""","").Replace(@""" ","");

            return new WestUpNycParsed(formUrl, sizeField);
        }

        public async Task<bool> SubmitAsync(AddressFields addressFields, WestUpNycParsed parsed, string email,
            string size, CancellationToken ct)
        {
            var getForm = await HttpClient.GetAsync(parsed.FormUrl, ct);
            var formContent = await getForm.ReadStringResultOrFailAsync("Can't access form", ct);
            var formDoc = new HtmlDocument();
            formDoc.LoadHtml(formContent);

            var csrfToken = formDoc.DocumentNode.SelectSingleNode("//input[@name='csrfmiddlewaretoken']").GetAttributeValue("value", "");
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"csrfmiddlewaretoken", csrfToken},
                {"$email", email},
                {"$first_name", addressFields.FirstName.Value},
                {"$last_name", addressFields.LastName.Value},
                {parsed.SizeField, size}
            });

            HttpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "iframe");
            HttpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
            HttpClient.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
            HttpClient.DefaultRequestHeaders.Add("origin","https://manage.kmail-lists.com");
            HttpClient.DefaultRequestHeaders.Add("referer",parsed.FormUrl);
            var postContent = await HttpClient.PostAsync(parsed.FormUrl, content, ct);
            var postResponse = await postContent.ReadStringResultOrFailAsync("Error on submission", ct);

            return postResponse.Contains("Confirm Your Email");
        }
    }
}