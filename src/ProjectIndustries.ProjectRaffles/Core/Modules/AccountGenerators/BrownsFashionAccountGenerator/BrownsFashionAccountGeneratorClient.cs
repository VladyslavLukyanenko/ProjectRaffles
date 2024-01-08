using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.BrownsFashionAccountGenerator
{
    public class BrownsFashionAccountGeneratorClient : IBrownsFashionAccountGeneratorClient
    {
        private readonly IHttpClientBuilder _builder;
        private HttpClient _httpClient;
        
        public BrownsFashionAccountGeneratorClient(IHttpClientBuilder builder)
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
            options.AllowAutoRedirect = true;
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.193 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("Accept","application/json, text/plain, */*");
                httpClient.DefaultRequestHeaders.Add("Accept-Language","en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
                httpClient.DefaultRequestHeaders.Add("Connection","keep-alive");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest","empty");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode","cors");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site","cross-site");
                httpClient.DefaultRequestHeaders.Add("sec-gpc","1");
            };
        }
        
        public async Task<bool> SubmitAccount(AddressFields addressFields, string userEmail, CancellationToken ct)
        {
            var registerUrl = "https://www.brownsfashion.com/dk/api/account/register";
            _httpClient.DefaultRequestHeaders.Add("origin","https://www.brownsfashion.com");
            _httpClient.DefaultRequestHeaders.Add("referer","https://www.brownsfashion.com/");
            
            _httpClient.DefaultRequestHeaders.Add("x-requested-with","XMLHttpRequest");

            var registerPayload = new
            {
                name = addressFields.FullName.Value,
                email = userEmail,
                username = userEmail,
                password = "ProjectRaffles!1)3!",
                receiveNewsletters = false
            };
            var json = JsonConvert.SerializeObject(registerPayload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var postContent = await _httpClient.PostAsync(registerUrl, content, ct);
            var postResponse = await postContent.Content.ReadAsStringAsync(ct);
            
            return postResponse.Contains(@"""isGuest"":false");
        }
        
    }
}