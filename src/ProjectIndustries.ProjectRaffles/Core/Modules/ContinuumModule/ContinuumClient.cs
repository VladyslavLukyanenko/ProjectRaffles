using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.ContinuumModule
{
    public class ContinuumClient : ModuleHttpClientBase, IContinuumClient
    {
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.AllowAutoRedirect = true;
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
            };
        }
        
        public async Task<string> GetProductAsync(string raffleUrl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleUrl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access site", ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;

            var title = node.Replace("\n", "").Replace("&ndash;", "|").Replace("B&amp;", "");
            return title;
        }

        public async Task<string> GetFormIdAsync(string raffleUrl, CancellationToken ct)
        {
            var raffleGet = await HttpClient.GetAsync(raffleUrl, ct);
            var raffleContent = await raffleGet.ReadStringResultOrFailAsync("Can't access site", ct);
            
            var doc = new HtmlDocument();
            doc.LoadHtml(raffleContent);

            var iFrameId = doc.DocumentNode.SelectSingleNode("//div[@class='pxFormGenerator']").GetAttributeValue("id", "");

            return iFrameId;
        }

        public Task<string> CraftJsonContent(AddressFields profile, string size, string email)
        { 
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
            };
            var jsonToSerialize = new ContinuumJson()
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Email = email,
                PhoneNumber = profile.PhoneNumber,
                Address = profile.AddressLine1,
                City = profile.City,
                State = profile.ProvinceId,
                ZipCode = profile.PostCode,
                Size = size
            };
            var data = JsonConvert.SerializeObject(jsonToSerialize, settings);

            return Task.FromResult(data);
        }

        public async Task<bool> SubmitAsync(string formId, string jsonContent, AddressFields profile, string email, CancellationToken ct)
        {
            var raffleContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"form_uuid", formId},
                {"formResponse", jsonContent},
                {"confirmationMail", email},
                {"is_pro", "false"}
            });
            
            var endpoint = "https://formbuilder.hulkapps.com/ajaxcall/formresponse";
            HttpClient.DefaultRequestHeaders.Add("referer",$"https://formbuilder.hulkapps.com/corepage/customform?id={formId}");

            var rafflePost = await HttpClient.PostAsync(endpoint, raffleContent, ct);
            if (!rafflePost.IsSuccessStatusCode) await rafflePost.FailWithRootCauseAsync("Error under submission", ct);

            return rafflePost.IsSuccessStatusCode;
        }
    }
}