using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WortherEsModule
{
    public class WortherEsClient : ModuleHttpClientBase, IWortherEsClient
    {
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.AllowAutoRedirect = false;
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("accept","*/*");
                httpClient.DefaultRequestHeaders.Add("accept-language","en-DK,en;q=0.9");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest","script");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode","no-cors");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site","cross-site");
                httpClient.DefaultRequestHeaders.Add("sec-gpc","1");
            };
        }

        public async Task<bool> SubmitAsync(string email, CancellationToken ct)
        {
            var bodyData = @"{""campaign-id"":""427577"",""email"":""" + email +
                           @""",""data"":[{""key"":""subscription"",""value"":""true""},{""key"":""utm_source"",""value"":""""},{""key"":""utm_medium"",""value"":""""},{""key"":""utm_campaign"",""value"":""""},{""key"":""utm_term"",""value"":""""},{""key"":""utm_content"",""value"":""""}],""site-session-id"":""" +
                           Guid.NewGuid().ToString() + @"""}";
            var body = bodyData.UriDataEscape();

            var dataUrl =
                $"https://api.flocktory.com/u_shaman/track-customer-actions.js?body={body}&callback=flock_jsonp_3";

            var getDataUrl = await HttpClient.GetAsync(dataUrl, ct);

            return getDataUrl.IsSuccessStatusCode;
        }
    }
}