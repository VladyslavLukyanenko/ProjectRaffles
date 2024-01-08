using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PanthersModule
{
  [EnableCaching]
  public interface IPanthersClient : IModuleHttpClient
  {
    [CacheOutput]
    Task<string> GetProductAsync(string raffleUrl, CancellationToken ct);
    [CacheOutput]
    Task<PanthersParsedRaffle> ParseRaffleAsync(string raffleurl, CancellationToken ct);
    Task<bool> SubmitAsync(PanthersSubmitPayload payload, CancellationToken ct);
  }
}