using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.Raffle43einhalbModule
{
  [EnableCaching]
  public interface IRaffle43einhalbClient : IModuleHttpClient
  {
    [CacheOutput]
    Task<string> GetProductNameAsync(string raffleUrl, CancellationToken ct);
    [CacheOutput]
    Task<string> ParseRaffleAsync(string raffleurl, string size, CancellationToken ct);
    
    Task<bool> SubmitAsync(AddressFields addressFields, string housenumber, string email, string bsid, string captcha, CancellationToken ct);
  }
}