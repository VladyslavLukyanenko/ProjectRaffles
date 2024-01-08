using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Apm.Api;
using HtmlAgilityPack;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.StarWarsCelebrationModule
{
    public class StarWarsCelebrationClient : ModuleHttpClientBase, IStarWarsCelebrationClient
    {
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("Accept-Language","en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
                httpClient.DefaultRequestHeaders.Add("Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                httpClient.DefaultRequestHeaders.Add("sec-gpc","1");
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

        public async Task<StarWarsCelebrationParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleUrl, ct);
            var pageContent = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

            var eventEditionRegexPattern = @"eventEditionId = "".*""";
            var eventEdition = new Regex(eventEditionRegexPattern).Match(pageContent).ToString().Replace("eventEditionId = ","").Replace(@"""","");

            
            var formCodeRegexPattern = @"formCode=.*&";
            var formCodeId = new Regex(formCodeRegexPattern).Match(pageContent).ToString().Replace("formCode=","").Replace("&","");
            
            var clientIdPattern = @"data-apigee-clientid="".*"" ";
            var clientId = new Regex(clientIdPattern).Match(pageContent).ToString().Replace(@"data-apigee-clientid=""","").Replace(@""" ","");
            
            return new StarWarsCelebrationParsed(eventEdition, formCodeId, clientId);
        }

        public async Task<bool> SubmitAsync(AddressFields address, StarWarsCelebrationParsed parsed, string email, string captcha, CancellationToken ct)
        {
            var firstNameString = @"{ key: ""FirstName"", value: """+ address.FirstName.Value +@""", values: null},";
            var lastNameString = @"{ key: ""LastName"", value: """+ address.LastName.Value +@""", values: null},";
            var emailString = @"{ key: ""Email"", value: """+ email +@""", values: null},";
            var recieveInformationString = @"{ key: ""ReceiveInformation"", value: ""I do not wish to receive such information"", values: [""I do not wish to receive such information""]},";
            var countryString = @"{ key: ""Country"", value: """+ address.CountryId.Value +@""", values: null},";
            var privacyString = @"{ key: ""privacyStatement"", value: ""true"", values: null},";
            var captchaString = @"{ key: ""CaptchaToken"", value: """+ captcha +@""", values: null}";
            
            
            var contentNotEscaped = @"eventEditionId: """ + parsed.EventId + @""", formCode: """ + parsed.FormId + @""", responses: [" +
                          firstNameString + lastNameString + emailString + recieveInformationString + countryString + privacyString + captchaString;

            var escapedContent = contentNotEscaped.Replace(@"""", @"\""");
            var contentString = @"{""query"": ""mutation {formResponseAdd(" + escapedContent + @"])}""}";

            HttpClient.DefaultRequestHeaders.Add("origin", "https://www.starwarscelebration.com");
            HttpClient.DefaultRequestHeaders.Add("referer","https://www.starwarscelebration.com/");
            HttpClient.DefaultRequestHeaders.Add("sec-fetch-dest","empty");
            HttpClient.DefaultRequestHeaders.Add("sec-fetch-mode","cors");
            HttpClient.DefaultRequestHeaders.Add("sec-fetch-site","cross-site");
            HttpClient.DefaultRequestHeaders.Add("x-clientid", parsed.ClientId);
            
            var content = new StringContent(contentString, Encoding.UTF8, "application/json");
            var endpoint = "https://api.reedexpo.com/graphql";
            var postContent = await HttpClient.PostAsync(endpoint, content, ct);

            return postContent.IsSuccessStatusCode;
        }
    }
}