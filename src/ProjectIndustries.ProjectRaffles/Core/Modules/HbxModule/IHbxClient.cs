using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.HbxModule
{
  [EnableCaching]
  public interface IHbxClient : IModuleHttpClient
  {
    [CacheOutput]
    Task<string> GetProductAsync(string raffleurl, CancellationToken ct);

    [CacheOutput]
    Task<HbxParsedRaffle> ParseProductAsync(string url, CancellationToken ct);

    Task<bool> SubmitAsync(AddressFields profile, string email, HbxParsedRaffle parsedRaffle, string captcha, string size,
      CancellationToken ct);
  }
}