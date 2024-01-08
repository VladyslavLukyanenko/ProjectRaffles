using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TravisScottModule
{
    public class TravisScottClient : ModuleHttpClientBase, ITravisScottClient
    {
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
            };
        }

        public async Task<TravisScottParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct)
        {
            var titoloBase = await HttpClient.GetAsync(raffleUrl, ct);
            var titoloContentBase = await titoloBase.ReadStringResultOrFailAsync("Can't access page", ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(titoloContentBase);

            var formUrl = doc.DocumentNode.SelectSingleNode("//form[@data-final='THANK YOU']").GetAttributeValue("action", "");

            var productId = doc.DocumentNode.SelectSingleNode("//input[@name='product_id']").GetAttributeValue("value", "");
            
            var kind = doc.DocumentNode.SelectSingleNode("//input[@name='kind']").GetAttributeValue("value", "");
            
            return new TravisScottParsed(formUrl, productId, kind);
        }

        public async Task<bool> SubmitAsync(string raffleUrl, string email, AddressFields address, TravisScottParsed parsed, string size, CancellationToken ct)
        {
            var craftedUrl = "?a=m" + $"&email={email}" + $"&first={address.FirstName}" + $"&last={address.LastName}" +
                             $"&zip={address.PostCode}" + $"&telephone={address.PhoneNumber}" +
                             $"&product_id={parsed.ProductId}" + $"&kind={parsed.Kind}" + $"&size={size}";
            var escapedUrl = craftedUrl.UriEscape();
            var fullUrl = parsed.FormEndpoint + escapedUrl;
            
            HttpClient.DefaultRequestHeaders.Add("referer", raffleUrl);
            var postEntry = await HttpClient.GetAsync(fullUrl, ct);
            var body = await postEntry.ReadStringResultOrFailAsync("Error on submission", ct);

            return body.Contains("thanks");
        }
    }
}