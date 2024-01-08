using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SuccezzModule
{
    public class SuccezzClient : ModuleHttpClientBase, ISuccezzClient
    {
        private readonly ICountriesService _countriesService;

        public SuccezzClient(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

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
            var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;

            var title = node.Replace("\n", "").Replace("&ndash;", "|").Replace("B&amp;", "");
            return title;
        }

        public async Task<string> GetFormIdAsync(string raffleUrl, CancellationToken ct)
        {
            var raffleGet = await HttpClient.GetAsync(raffleUrl, ct);
            var raffleContent = await raffleGet.ReadStringResultOrFailAsync("Can't access page", ct);
            
            var doc = new HtmlDocument();
            doc.LoadHtml(raffleContent);

            var iFrameId = doc.DocumentNode.SelectSingleNode("//div[@class='pxFormGenerator']").GetAttributeValue("id", "");

            return iFrameId;
        }

        public Task<string> CraftJsonContent(AddressFields profile, string email, string size)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
            };
            
            var country = _countriesService.GetCountryName(profile.CountryId)
                .Replace(" (" + profile.CountryId + ")", "");

            var address2 = "";
            if (profile.AddressLine2 != null) address2 = profile.AddressLine2.Value;
            
            Dictionary<string, string>[] addressDictionary = new[] 
            {
                new Dictionary<string, string>() { {"Address line 1", profile.AddressLine1.Value} },
                new Dictionary<string, string>() { {"Address line 2", address2} },
                new Dictionary<string, string>() { {"City", profile.City.Value} },
                new Dictionary<string, string>() { {"Province", profile.ProvinceId.Value} },
                new Dictionary<string, string>() { {"Zip code", profile.PostCode.Value} },
                new Dictionary<string, string>() { {"Country", country} },
            };

            var jsonToSerialize = new SuccezzJson()
            {
                FirstName = profile.FirstName.Value,
                LastName = profile.LastName.Value,
                Email = email,
                PhoneNumber = profile.PhoneNumber.Value,
                Size = size,
                Address = addressDictionary
            };
            var data = JsonConvert.SerializeObject(jsonToSerialize, settings);

            return Task.FromResult(data);
        }

        public async Task<bool> SubmitAsync(string formId, string jsonContent, CancellationToken ct)
        {
            var raffleContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"form_uuid", formId},
                {"formResponse", jsonContent},
                {"confirmationMail", ""},
                {"is_pro", "true"}
            });
            
            var endpoint = "https://formbuilder.hulkapps.com/ajaxcall/formresponse";
            HttpClient.DefaultRequestHeaders.Add("referer",$"https://formbuilder.hulkapps.com/corepage/customform?id={formId}");

            var rafflePost = await HttpClient.PostAsync(endpoint, raffleContent, ct);
            if (!rafflePost.IsSuccessStatusCode) await rafflePost.FailWithRootCauseAsync("Error under submission", ct);

            return rafflePost.IsSuccessStatusCode;
        }
    }
}