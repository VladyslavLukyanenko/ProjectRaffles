using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.FenomAccountGenerator
{
    public class FenomAccountGeneratorClient : IFenomAccountGeneratorClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private readonly IHttpClientBuilder _builder;
        private HttpClient _httpClient;
        
        public FenomAccountGeneratorClient(IHttpClientBuilder builder)
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
            };
        }
        
        public async Task<bool> SubmitAccount(AddressFields addressFields, string email, string gender)
        {
            var registerUrl = "https://www.fenom.com/en/authentication?create_account=1";
            
            //get prestashop cookies
            var prestaCookies = await _httpClient.GetAsync(registerUrl);
            if (!prestaCookies.IsSuccessStatusCode)
                await prestaCookies.FailWithRootCauseAsync("Can't access registration page");
            
            var registerContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"id_gender", gender},
                {"firstname", addressFields.FirstName.Value},
                {"lastname", addressFields.LastName.Value},
                {"email", email},
                {"password","ProjectRaffles!1)3!"},
                {"birthday", ""},
                {"psgdpr", "1"},
                {"submitCreate", "1"}
            });

            _httpClient.DefaultRequestHeaders.Add("origin","https://www.fenom.com");
            _httpClient.DefaultRequestHeaders.Add("referer",registerUrl);
            var postRegistration = await _httpClient.PostAsync(registerUrl, registerContent);
            if(!postRegistration.IsSuccessStatusCode) await postRegistration.FailWithRootCauseAsync($"Can't submit user with email: {email}");

            return postRegistration.IsSuccessStatusCode;
        }
    }
}