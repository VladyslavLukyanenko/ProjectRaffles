using System;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.IncuModule
{
    public class IncuClient : ModuleHttpClientBase, IIncuClient
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

        public async Task<string> GetProductAsync(string url, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(url, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;
            var title = node.Replace("Chmielna20.pl - ", "");

            return title;
        }

        public async Task<IncuParsedRaffle> ParseRaffleAsync(string url, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(url, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var formUrl = doc.DocumentNode.SelectSingleNode("//form[@id='raffle-form']")
                .GetAttributeValue("action", "");

            var sheet = doc.DocumentNode.SelectSingleNode("//input[@name='sheet_name']").GetAttributeValue("value", "");

            return new IncuParsedRaffle(formUrl, sheet);
        }

        public Task<string> CreateUrlAsync(AddressFields profile, string email, IncuParsedRaffle parsed, string captcha, string size)
        {
            var createParams = "?" + $"first_name={profile.FirstName.Value}" + $"&last_name=={profile.LastName.Value}" +
                               $"&email_address={email}" + $"&phone_number={profile.PhoneNumber.Value}" +
                               $"&shoe_size={size}" + $"&sheet_name={parsed.Sheet}" +
                               $"&g-recaptcha-response={captcha}";

            var escapeParams = createParams.UriEscape();
            var finalUrl = parsed.SubmitUrl + escapeParams;

            return Task.FromResult(finalUrl);
        }

        public async Task<bool> SubmitAsync(string createdUrl, CancellationToken ct)
        {
            var raffleResponse = await HttpClient.GetAsync(createdUrl, ct);
            var resultHtml = await raffleResponse.ReadStringResultOrFailAsync("Error on submission", ct);
            
            
            if (!resultHtml.Contains("success")) await raffleResponse.ReadStringResultOrFailAsync("Error on submission",
                ct);

            return resultHtml.Contains("success");
        }
    }
}