using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.GoogleFormsModule
{
    public class GoogleFormsClient : ModuleHttpClientBase, IGoogleFormsClient
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
        
        public async Task<bool> SubmitAsync(List<KeyValuePair<string,string>> data, string sourceUrl, CancellationToken ct)
        {
            var getSite = await HttpClient.GetAsync(sourceUrl, ct);
            var body = await getSite.Content.ReadAsStringAsync(ct);
            
            var doc = new HtmlDocument();
            doc.LoadHtml(body);
      
            var fvv = doc.DocumentNode.SelectSingleNode("//input[@name='fvv']").GetAttributeValue("value", "");
            var draftResponse = doc.DocumentNode.SelectSingleNode("//input[@name='draftResponse']").GetAttributeValue("value", "").Replace("&quot;",@"""").Replace("\n","");
            var fbzx = doc.DocumentNode.SelectSingleNode("//input[@name='fbzx']").GetAttributeValue("value", "");
            var pageHistory = doc.DocumentNode.SelectSingleNode("//input[@name='pageHistory']").GetAttributeValue("value", "");
            
            HttpClient.DefaultRequestHeaders.Add("referer", sourceUrl + $"?fbzx={fbzx}");
      
            data.Add(new KeyValuePair<string, string>("fvv",fvv));
            data.Add(new KeyValuePair<string, string>("draftResponse", draftResponse));
            data.Add(new KeyValuePair<string, string>("fbzx", fbzx));
            data.Add(new KeyValuePair<string, string>("pageHistory", pageHistory));
      
            var content = new FormUrlEncodedContent(data);

            var endpoint = sourceUrl.Replace("viewform", "formResponse");

            var postEntry = await HttpClient.PostAsync(endpoint, content, ct);
            if(!postEntry.IsSuccessStatusCode) await postEntry.FailWithRootCauseAsync("Error on submission", ct);

            return postEntry.IsSuccessStatusCode;
        }
    }
}