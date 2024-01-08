using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.MahaAccountGenerator;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.WoeiAccountGenerator
{
    [EnableCaching]
    public interface IWoeiAccountGeneratorClient
    {
        void GenerateHttpClient();
        [CacheOutput]
        Task<WoeiAccountGeneratorParsed> GetCountriesAsync(CancellationToken ct);

        Task<string> GetCountryRegionIdAsync(string country, string province, CancellationToken ct);

        Task<string> GetPhoneCodeAsync(string country, CancellationToken ct);

        Task<string> SubmitAccountAsync(AddressFields addressFields, string email, string type, string houseNumber,
            string countryId, string provinceId, string phonecode, CancellationToken ct);

        Task<bool> SubmitCaptchaAsync(string authtoken, string captcha, CancellationToken ct);
    }
}