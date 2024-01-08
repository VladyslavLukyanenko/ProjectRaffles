using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Apm.Api;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.CorporateGotEmModule
{
    public class CorporateGotEmClient : ModuleHttpClientBase, ICorporateGotEmClient
    {
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("Accept-Language","en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
                httpClient.DefaultRequestHeaders.Add("Connection","keep-alive");
                httpClient.DefaultRequestHeaders.Add("sec-gpc","1");
            };
        }
        
        public async Task<string> GetProductAsync(string raffleurl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleurl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access site", ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;

            var title = node.Replace("\n", "").Replace("&ndash;", "|").Replace("B&amp;", "");
            return title;
        }

        public async Task<CorporateGotEmParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct)
        {
            var getUrl = await HttpClient.GetAsync(raffleUrl, ct);
            var getContent = await getUrl.ReadStringResultOrFailAsync("Can't access site", ct);

            var klaviyoRegex = new Regex(@"klaviyo-form-.{1,10}""").Match(getContent).ToString().Replace(@"""", "")
                .Replace("klaviyo-form-", "");

            HttpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest","empty");
            HttpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode","cors");
            HttpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site","cross-site");
            
            var allFormsUrl = "https://fast.a.klaviyo.com/forms/api/v3/full-forms?company_id=HdQwts";
            var getForms = await HttpClient.GetAsync(allFormsUrl, ct);
            var allForms = await getForms.ReadStringResultOrFailAsync("Can't get all forms", ct);

            dynamic formsParser = JArray.Parse(allForms);

            var formDictionary = new Dictionary<string, dynamic>();

            foreach (var formObject in formsParser)
            {
                string formId = formObject.form_id;

                formDictionary.Add(formId, formObject);
            }

            formDictionary.TryGetValue(klaviyoRegex, out dynamic currentForm);

            string formVersion = (string) currentForm["live_form_versions"][0]["form_version_id"];
            string formName = currentForm.name;

            string formGId =
                (string) currentForm["live_form_versions"][0]["views"][0]["columns"][0]["rows"][5]["components"][1]
                    ["action"]["list_id"];
            
            return new CorporateGotEmParsed(klaviyoRegex, formVersion, formName, formGId);
        }

        public async Task<bool> SubmitAsync(AddressFields addressFields, CorporateGotEmParsed parsed, string email, string size, CancellationToken ct)
        {
            var timeOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).ToString().Replace(":00:00","");

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"g", parsed.ListId},
                {"$fields", $"$source,$email,$first_name,$last_name,{parsed.FormName}_Zip,{parsed.FormName}_Size,$consent_method,$consent_form_id,$consent_form_version,services"},
                {"$list_fields", ""},
                {"$timezone_offset", timeOffset},
                {"$source", parsed.FormName},
                {"$email", email},
                {"$first_name", addressFields.FirstName.Value},
                {"$last_name", addressFields.LastName.Value},
                {$"{parsed.FormName}_Zip", addressFields.PostCode.Value},
                {$"{parsed.FormName}_Size", size},
                {"$consent_method", "Klaviyo Form"},
                {"$consent_form_id", parsed.FormId},
                {"$consent_form_version", parsed.FormVersion},
                {"services", @"{""shopify"":{""source"":""form""}}"}
            });

            var endpoint = "https://a.klaviyo.com/ajax/subscriptions/subscribe";
            var post = await HttpClient.PostAsync(endpoint, content, ct);
            var postContent = await post.ReadStringResultOrFailAsync($"Error on submission for email {email}", ct);

            if (!postContent.Contains(@"""success"": true"))
                await post.FailWithRootCauseAsync($"Error with email {email}", ct);
            
            return postContent.Contains(@"""success"": true");
        }
    }
}