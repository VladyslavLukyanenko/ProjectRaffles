using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProjectIndustries.ProjectRaffles.Core.Services;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.SneakersDelightAccountGenerator
{
    public class SneakersDelightAccountGeneratorClient : ISneakersDelightAccountGeneratorClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private readonly IHttpClientBuilder _builder;
        private HttpClient _httpClient;
        private readonly IBirthdayProviderService _birthdayProvider;
        private readonly IStringUtils _stringUtils;
    
        public SneakersDelightAccountGeneratorClient(IHttpClientBuilder builder, IBirthdayProviderService birthdayProvider, IStringUtils stringUtils)
        {
            _builder = builder;
            _birthdayProvider = birthdayProvider;
            _stringUtils = stringUtils;
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

        public async Task<SneakersDelightAccountGeneratorParsed> ParseLoginAsync(CancellationToken ct)
        {
            var getPage = await _httpClient.GetAsync("https://sneakersdelight.store/customer/account/login/", ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't get registration page");
            
            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var formKey = doc.DocumentNode.SelectSingleNode("//input[@name='form_key']").GetAttributeValue("value", "");
            var referer = doc.DocumentNode.SelectSingleNode("//input[@name='referer']").GetAttributeValue("value", "");
            
            return new SneakersDelightAccountGeneratorParsed(formKey, referer);
        }

        public async Task<bool> SubmitAccountAsync(SneakersDelightAccountGeneratorParsed parsed,
            AddressFields addressFields, string email, string gender, string captcha,
            CancellationToken ct)
        {
            var dob = await _birthdayProvider.GenerateDob();
            
            var formBoundary = await _stringUtils.GenerateRandomStringAsync(16);
            var content = new MultipartFormDataContent("----WebKitFormBoundary" + formBoundary)
            {
                {new StringContent(parsed.FormKey), "form_key"},
                {new StringContent(parsed.Referer), "referer"},
                {new StringContent(""), "success_url"},
                {new StringContent(""), "error_url"},
                {new StringContent(addressFields.FirstName), "firstname"},
                {new StringContent(addressFields.LastName), "lastname"},
                {new StringContent(email), "email"},
                {new StringContent(dob), "dob"},
                {new StringContent(gender), "gender"},
                {new StringContent("ProjectRaffles!1)3!"), "password"},
                {new StringContent("ProjectRaffles!1)3!"), "password_confirmation"},
                {new StringContent(captcha), "minty_invisible_token"}
            };
            var endpoint = "https://sneakersdelight.store/customer/account/createpost/";
            var postContent = await _httpClient.PostAsync(endpoint, content, ct);
            if (!postContent.IsSuccessStatusCode) await postContent.FailWithRootCauseAsync("Can't submit account");

            return postContent.IsSuccessStatusCode;
        }
    }
}