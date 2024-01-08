using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.OffTheHookModule
{
  [EnableCaching]
  public interface IOffTheHookClient : IModuleHttpClient
  {
    [CacheOutput]
    Task<string> GetProductAsync(string raffleurl, CancellationToken ct);

    [CacheOutput]
    Task<OffTheHookParsedRaffle> ParseSiteAsync(string raffleUrl, CancellationToken ct);

    Task<bool> SubmitAsync(AddressFields profile, string email, OffTheHookParsedRaffle parsedRaffle, string size,
      string instagram, CancellationToken ct);
  }
}