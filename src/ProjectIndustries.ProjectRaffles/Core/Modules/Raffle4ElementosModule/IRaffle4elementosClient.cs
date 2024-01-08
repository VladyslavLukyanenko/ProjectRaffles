using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.Raffle4ElementosModule
{
  [EnableCaching]
  public interface IRaffle4elementosClient : IModuleHttpClient
  {
    [CacheOutput]
    Task<string> GetProductAsync(string raffleurl, CancellationToken ct);
    [CacheOutput]
    Task<Raffle4elementosParsed> ParseRaffleAsync(string raffleurl, CancellationToken ct);
    Task<bool> SubmitAsync(Raffle4elementosSubmitPayload payload, CancellationToken ct);
  }
}