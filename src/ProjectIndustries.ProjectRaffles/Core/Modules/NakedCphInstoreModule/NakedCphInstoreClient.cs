using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Modules.NakedCphModule;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NakedCphInstoreModule
{
    public class NakedCphInstoreClient : ModuleHttpClientBase, INakedCphInstoreClient
    {
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("accept-language","en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-dest","document");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-mode","navigate");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-site","cross-site");
                httpClient.DefaultRequestHeaders.Add("sec-gpc","1");
            };
        }
        
        public async Task<bool> SubmitAsync(AddressFields addressFields, string email, string instagramHandle, string raffleTag,
            string size, CancellationToken ct)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"tags[]", raffleTag},
                {"token", "c812c1ff-2a5a0fe-efad139-d754416-71e1e60-2ce"},
                {"rule_email", email},
                {"fields[Raffle.Instagram Handle]", instagramHandle},
                {"fields[Raffle.Phone Number]", addressFields.PhoneNumber.Value},
                {"fields[Raffle.First Name]", addressFields.FirstName.Value},
                {"fields[Raffle.Last Name]:", addressFields.LastName.Value},
                {"fields[Raffle.Address]", addressFields.AddressLine1.Value},
                {"fields[Raffle.Postal Code]", addressFields.PostCode.Value},
                {"fields[Raffle.City]", addressFields.City.Value},
                {"fields[Raffle.Shoe Size]", size},
                {"fields[Raffle.Country]", addressFields.CountryId.Value}
            });

            HttpClient.DefaultRequestHeaders.Add("origin","https://www.nakedcph.com");
            HttpClient.DefaultRequestHeaders.Add("referer","https://www.nakedcph.com/");
            var url = "https://app.rule.io/subscriber-form/subscriber";
            var signup = await HttpClient.PostAsync(url, content, ct);
            var signupContent = await signup.ReadStringResultOrFailAsync("Error on submission", ct);
      
            if(!signupContent.Contains("YOUR REGISTRATION WAS SUCCESSFUL")) await signup.FailWithRootCauseAsync("Submission error", ct);
            return signupContent.Contains("YOUR REGISTRATION WAS SUCCESSFUL");
        }
    }
}