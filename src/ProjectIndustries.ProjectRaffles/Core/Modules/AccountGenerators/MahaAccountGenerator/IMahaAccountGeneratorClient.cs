using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.MahaAccountGenerator
{
    [EnableCaching]
    public interface IMahaAccountGeneratorClient
    {
        void GenerateHttpClient();
        [CacheOutput]
        Task<MahaAccountGeneratorParsed> GetCountriesAsync(CancellationToken ct);

        Task<string> GetCountryRegionIdAsync(string country, string province, CancellationToken ct);

        Task<string> GetPhoneCodeAsync(string country, CancellationToken ct);

        Task<string> SubmitAccountAsync(AddressFields addressFields, string email, string type, string houseNumber,
            string countryId, string provinceId, string phonecode, CancellationToken ct);

        Task<bool> SubmitCaptchaAsync(string authtoken, string captcha, CancellationToken ct);
    }
}