using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.StreetmachineAccountGenerator
{
    public class StreetmachineAccountGeneratorClient : IStreetmachineAccountGeneratorClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private readonly IHttpClientBuilder _builder;
        private HttpClient _httpClient;

        public StreetmachineAccountGeneratorClient(IHttpClientBuilder builder)
        {
            _builder = builder;
        }
        
        public void GenerateHttpClient()
        {
            _httpClient = _builder.WithConfiguration(ConfigureHttpClient)
                .Build();
        }

        private void ConfigureHttpClient(HttpClientOptions options)
        {
            options.CookieContainer = _cookieContainer;
            options.AllowAutoRedirect = true;
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
            };
        }

        public async Task GetCookies(CancellationToken ct)
        {
            //get ip for bbc cookie
            var getIp = await _httpClient.GetAsync("https://api.ipify.org", ct);
            var ip = await getIp.ReadStringResultOrFailAsync("Can't get IP");
            
            var bbcIpCookie = new Cookie("bbc", ip) {Domain = "streetmachine.com"};
            _cookieContainer.Add(bbcIpCookie);
            
            //try to skip queue
            var unixTime = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 20*1000}"; //say we have been in queue for 20sec
            var queueCookie = new Cookie("queue", unixTime) {Domain = "streetmachine.com"};
            _cookieContainer.Add(queueCookie);
            
            //get site so we get PHPSESSID cookie
            await _httpClient.GetAsync("https://www.streetmachine.com", ct);
        }

        public async Task<bool> SubmitAccountAsync(string email, AddressFields addressFields, string captcha,
            CancellationToken ct)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"formid","register"},
                {"g-recaptcha-response", captcha},
                {"first_name", addressFields.FirstName.Value},
                {"surname", addressFields.LastName.Value},
                {"email", email},
                {"password", "ProjectRaffles!1)3!"},
                {"password_repeat", "ProjectRaffles!1)3!"}
            });

            _httpClient.DefaultRequestHeaders.Add("Origin","https://www.streetmachine.com");
            _httpClient.DefaultRequestHeaders.Add("Referer","https://www.streetmachine.com/register");
            _httpClient.DefaultRequestHeaders.Add("sec-gpc", "1");
            var accountEndpoint = "https://www.streetmachine.com/account";
            var postAccount = await _httpClient.PostAsync(accountEndpoint, content, ct);
            var postBody = await postAccount.ReadStringResultOrFailAsync("Can't submit account");
            
            return postBody.Contains("Du kan opdatere dine personlige informationer");
        }
    }
}