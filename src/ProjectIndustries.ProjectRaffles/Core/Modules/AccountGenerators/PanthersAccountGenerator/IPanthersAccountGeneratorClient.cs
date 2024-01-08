using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.PanthersAccountGenerator
{
  [EnableCaching]
  public interface IPanthersAccountGeneratorClient
  {
    void GenerateHttpClient();
    [CacheOutput]
    Task<PanthersAccountGeneratorsParsed> GetCountriesAsync(CancellationToken ct);

    //Task<string> GetCountryRegionIdAsync(string country, string province, CancellationToken ct);
    
    Task<string> GetPhoneCodeAsync(string country, CancellationToken ct);

    Task<string> SubmitAccountAsync(AddressFields addressFields, string email, string type, string houseNumber,
      string country, string phoneCode, CancellationToken ct);

    Task<bool> SubmitCaptchaAsync(string authtoken, string captcha, CancellationToken ct);
  }
}