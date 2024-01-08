using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.CopdateModule
{
    public class CopdateClient : ModuleHttpClientBase, ICopdateClient
    {
        private readonly IStringUtils _stringUtils;

        public CopdateClient(IStringUtils stringUtils)
        {
            _stringUtils = stringUtils;
        }
        
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("origin","copdate.com");
            };
        }

        public async Task<string> GetProductAsync(string raffleurl, CancellationToken ct)
        {
            var getRaffle = await HttpClient.GetAsync(raffleurl, ct);
            var raffleContent = await getRaffle.ReadStringResultOrFailAsync("Can't access raffle page", ct);
            
            var doc = new HtmlDocument();
            doc.LoadHtml(raffleContent);

            var product = doc.DocumentNode.SelectSingleNode("//input[@name='product']").GetAttributeValue("value", "");

            return product;
        }

        public async Task<bool> VerifyAccountAsync(Account account, AddressFields profile, CancellationToken ct)
        {
            var baseUrl = "https://admin.copdate.com/application/services/index?service=checkUser";

            var urlParams = $"&email={account.Email}" + $"&first-name={profile.FirstName.Value}" +
                            $"&last-name={profile.LastName.Value}";

            var escapedUrl = urlParams.UriEscape();

            var endpoint = baseUrl + escapedUrl;

            var checkAccount = await HttpClient.GetAsync(endpoint, ct);
            var content = await checkAccount.ReadStringResultOrFailAsync("Error on verifying account", ct);
            if (content.Contains(@"""result"":false")) await checkAccount.FailWithRootCauseAsync("This email/name combo isn't valid", ct);

            return checkAccount.IsSuccessStatusCode;
        }

        public async Task<CopdateParsedRaffle> ParseRaffleAsync(string raffleUrl, CancellationToken ct)
        {
            var getRaffle = await HttpClient.GetAsync(raffleUrl, ct);
            var raffleContent = await getRaffle.ReadStringResultOrFailAsync("Can't access raffle page", ct);
            
            var doc = new HtmlDocument();
            doc.LoadHtml(raffleContent);
            
            var gId = doc.DocumentNode.SelectSingleNode("//input[@name='g']").GetAttributeValue("value", "");
            var entry = doc.DocumentNode.SelectSingleNode("//input[@id='entry']").GetAttributeValue("value", "");
            var entryId = doc.DocumentNode.SelectSingleNode("//input[@id='entryId']").GetAttributeValue("value", "");
            var product = doc.DocumentNode.SelectSingleNode("//input[@id='prod']").GetAttributeValue("value", "");
            var store = doc.DocumentNode.SelectSingleNode("//input[@id='store']").GetAttributeValue("value", "");
            
            return new CopdateParsedRaffle(gId, entry, entryId, product, store);
        }

        public async Task<bool> SubmitAsync(CopdateParsedRaffle parsedRaffle, Account account, AddressFields profile, string size, string gender, string raffleurl, CancellationToken ct)
        {
            HttpClient.DefaultRequestHeaders.Add("referer", raffleurl);
            
            var generateCharacters = await _stringUtils.GenerateRandomStringAsync(5);
            var dumpEmail = generateCharacters + profile.FirstName.Value + "@dump.com";

            var timeOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).ToString().Replace(":00:00","");
            
            var random = new Random();

            var timeOnPage = random.Next(60, 180).ToString();

            var generatedCaptchaLetters = await _stringUtils.GenerateRandomStringAsync(21);
            var captcha =
                @"03AGdBq27TRv-En2V8dECxmZ5Byl31m60vytbPIdvdz7Thok3XR8gZKZBrGjNlTw5vcpPYA3btUhy9N8cjbMhz67o9m9UHfLGqYGsl9TyjhqjPhAWlSdyL0D9QfsYvw2MZibaoBqAFpdfUY5F67Z6e0iRxLHqOspC69gQYiIPMiGU7226aF6hY5UeeaQJjcxMTuYtowEZ2lXLbP6SFSW9TCOSGxd5GLm0IMp_vKzgyB4JeJdshQRS63pStA1sHRPOkMFLe81xq3bH2n4-1a30q3Q9mxFL_F307tfuqKrCBSEnaJUvRQ69fQnwG18fXbu3rYTkH1VYBivxpSD0PylRiMe4Q9AVpyACQfF8eOdD6hKldoMA8UQk95FSLrnG2197ks-srL6dsmbu7OTRlOY-Us2lAGWYr5KUImSpHcv952BOZSYCBc1NxglQz9z3ZD1lgPLuxKEGeZ46_" + generatedCaptchaLetters;
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"g", parsedRaffle.GId},
                {"$fields", "phone,gender,size,time,_email,$source"},
                {"page_slug", parsedRaffle.Entry},
                {"entryId", parsedRaffle.EntryId},
                {"page_url", "https://list.copdate.com/"},
                {"product", parsedRaffle.Product},
                {"store", parsedRaffle.Store},
                {"time", timeOnPage},
                {"first_name", profile.FirstName.Value},
                {"last_name", profile.LastName.Value},
                {"_email", account.Email},
                {"email", dumpEmail},
                {"phone", profile.PhoneNumber},
                {"gender", gender},
                {"size", size},
                {"g-recaptcha-response", captcha},
                {"$timezone_offset", timeOffset},
                {"$source", "$embed"}
            });
            var endpoint = "https://manage.kmail-lists.com/ajax/subscriptions/subscribe";

            var post = await HttpClient.PostAsync(endpoint, content, ct);
            var postResponse = await post.ReadStringResultOrFailAsync("Error on submission", ct);

            return postResponse.Contains(@"""success"": true");
        }
    }
}