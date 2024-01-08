using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.SnkrKuwaitAccountGenerator
{
    public class SnkrKuwaitAccountGeneratorClient : ISnkrKuwaitAccountGeneratorClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private readonly IHttpClientBuilder _builder;
        private HttpClient _httpClient;

        public SnkrKuwaitAccountGeneratorClient(IHttpClientBuilder builder)
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
            options.AllowAutoRedirect = false;
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
            };
        }

        public async Task<bool> SubmitAccount(AddressFields addressFields, string email, CancellationToken ct)
        {
            var registerUrl = "https://www.snkr.com.kw/customer/account/create/";

            var baseRegistrationSite = await _httpClient.GetAsync(registerUrl, ct);
            var baseRegistrationContent =
                await baseRegistrationSite.ReadStringResultOrFailAsync("Can't access registration page");

            var doc = new HtmlDocument();
            doc.LoadHtml(baseRegistrationContent);
            var formkey = doc.DocumentNode.SelectSingleNode("//input[@name='form_key']").GetAttributeValue("value", "");

            var registerContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"form_key", formkey},
                {"success_url", ""},
                {"error_url", ""},
                {"firstname", addressFields.FirstName.Value},
                {"middlename", ""},
                {"lastname", addressFields.LastName.Value},
                {"dob", ""},
                {"gender", ""},
                {"mobilenumber", ""},
                {"email", email},
                {"password", "ProjectRaffles!1)3!"},
                {"password_confirmation", "ProjectRaffles!1)3!"},
                {"persistent_remember_me", "on"}
            });

            _httpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
            _httpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
            _httpClient.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
            _httpClient.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
            _httpClient.DefaultRequestHeaders.Add("origin", "https://www.snkr.com.kw");
            _httpClient.DefaultRequestHeaders.Add("referer", registerUrl);

            var endpoint = "https://www.snkr.com.kw/customer/account/createpost/";
            var postRegistration = await _httpClient.PostAsync(endpoint, registerContent, ct);

            string headerLocation = "";
            var headers = postRegistration.Headers;
            IEnumerable<string> values;
            if (headers.TryGetValues("location", out values))
            {
                headerLocation = values.First();
            }

            if (!headerLocation.Equals("https://www.snkr.com.kw/customer/account/"))
                await postRegistration.FailWithRootCauseAsync("Error on submission");

            return
                headerLocation.Equals(
                    "https://www.snkr.com.kw/customer/account/"); //we check headers for success because site glitchy
        }
    }
}