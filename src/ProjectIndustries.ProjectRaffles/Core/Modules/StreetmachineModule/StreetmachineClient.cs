using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.StreetmachineModule
{
    public class StreetmachineClient : ModuleHttpClientBase, IStreetmachineClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();

        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.AllowAutoRedirect = true;
            options.CookieContainer = _cookieContainer;
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.193 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
            };
        }

        public async Task<string> LoginAsync(Account account, CancellationToken ct)
        {
            //get ip for bbc cookie
            var getIp = await HttpClient.GetAsync("https://api.ipify.org", ct);
            var ip = await getIp.ReadStringResultOrFailAsync("Can't get IP", ct);
            
            var bbcIpCookie = new Cookie("bbc", ip) {Domain = "streetmachine.com"};
            _cookieContainer.Add(bbcIpCookie);
            
            //try to skip queue
            var unixTime = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 20*1000}"; //say we have been in queue for 20sec
            var queueCookie = new Cookie("queue", unixTime) {Domain = "streetmachine.com"};
            _cookieContainer.Add(queueCookie);
            
            //get site so we get PHPSESSID cookie
            var getSite = await HttpClient.GetAsync("https://www.streetmachine.com", ct);

            var loginPostContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"formid", "login"},
                {"email", account.Email},
                {"password", account.Password}
            });

            var loginPost = await HttpClient.PostAsync("https://www.streetmachine.com/login", loginPostContent, ct);
            var loginContent = await loginPost.ReadStringResultOrFailAsync("Can't login", ct);

            if (!loginContent.Contains("logout")) await loginPost.FailWithRootCauseAsync("Error on login", ct);

            return "";
        }

        public async Task<StreetmachineParsed> ParseSizesAsync(string raffleUrl, string size, CancellationToken ct)
        {
            var getRaffle = await HttpClient.GetAsync(raffleUrl, ct);
            var raffleHtml = await getRaffle.ReadStringResultOrFailAsync("Can't access raffle page", ct);

            //get the terms checkbox
            var doc = new HtmlDocument();
            doc.LoadHtml(raffleHtml);

            var termsCheckbox = doc.DocumentNode
                .SelectSingleNode("//input[@label='I HAVE READ AND AGREE TO THE TERMS']").GetAttributeValue("value","");
            
            //size etc.
            var sizeRegexPattern = @"<option value=""\d{1,7}"">"+size+@"<\/option>";
            var sizeRegex = new Regex(sizeRegexPattern);
            var sizeId = sizeRegex.Match(raffleHtml).ToString().Replace(@"<option value=""", "")
                .Replace(@""">" + size + @"</option>", "");
            
            return new StreetmachineParsed(termsCheckbox, sizeId);
        }

        public async Task<bool> SubmitAsync(StreetmachineParsed parsed, string captcha, string raffleUrl, CancellationToken ct)
        {
            var rafflePostContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"formid", "draw"},
                {"g-recaptcha-response", captcha},
                {"item_pid", parsed.SizeId},
                {parsed.TermsCheckbox, "on"}
            });

            var rafflePost = await HttpClient.PostAsync(raffleUrl, rafflePostContent, ct);
            var body = await rafflePost.ReadStringResultOrFailAsync("Error on submission", ct);

            return body.Contains("Du deltager i denne udtrækning.");
        }
    }
}