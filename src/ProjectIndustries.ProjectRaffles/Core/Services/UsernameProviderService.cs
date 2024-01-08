using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public class UsernameProviderService : IUsernameProviderService
    {
        private readonly HttpClient _httpClient;
        
        public UsernameProviderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<string> GenerateUsernameAsync(CancellationToken ct)
        {
            var usernameUrl = "https://randomuser.me/api/?inc=login";

            var get = await _httpClient.GetAsync(usernameUrl, ct);
            var body = await get.Content.ReadAsStringAsync(ct);

            var userNameObject = body.Replace("[", "").Replace("]", "");
            dynamic randomLoginDetails = JObject.Parse(userNameObject);
            var username = randomLoginDetails.results.login.username;

            return username;
        }
    }
}