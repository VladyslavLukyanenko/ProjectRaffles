using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AwolModule
{
  [EnableCaching]
  public interface IAwolClient : IModuleHttpClient
  {
    [CacheOutput]
    Task<AwolParsedRaffle> ParseRaffleAsync(string raffleUrl, CancellationToken ct);
    Task<bool> SubmitAsync(AwolSubmitPayload payload, CancellationToken ct);
  }
}