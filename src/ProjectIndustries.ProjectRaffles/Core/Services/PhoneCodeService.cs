using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public class PhoneCodeService : IPhoneCodeService
    {
        private readonly HttpClient _httpClient;
    
        public PhoneCodeService(IHttpClientBuilder builder)
        {
            _httpClient = builder.WithConfiguration(ConfigureHttpClient)
                .Build();
        }

        private void ConfigureHttpClient(HttpClientOptions options)
        {
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
            };
        }
        
        public async Task<string> GetPhoneCodeAsync(string country, CancellationToken ct)
        {
            var apiUrl = $"https://restcountries.eu/rest/v2/alpha/{country}";
      
            var getPage = await _httpClient.GetAsync(apiUrl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't get phone code");

            var json = JObject.Parse(body);
            string countryId = (string) json["callingCodes"][0];

            return countryId;
        }
    }
}