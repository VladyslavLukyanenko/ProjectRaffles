using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PhenomModule
{
  [EnableCaching]
  public interface IPhenomClient : IModuleHttpClient
  {
    [CacheOutput]
    Task<PhenomParsedRaffle> ParseRaffleAsync(string raffleurl, CancellationToken ct);
    Task<bool> SubmitAsync(PhenomSubmitPayload payload, CancellationToken ct);
  }
}