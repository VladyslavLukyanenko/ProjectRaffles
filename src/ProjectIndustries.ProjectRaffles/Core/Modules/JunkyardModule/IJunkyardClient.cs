using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.JunkyardModule
{
  [EnableCaching]
  public interface IJunkyardClient : IModuleHttpClient
  {
    [CacheOutput]
    Task<string> GetProductAsync(string url, CancellationToken ct);
    
    [CacheOutput]
    Task<JunkyardParsedRaffleFields> ParseRaffleAsync(string size, string raffleurl, CancellationToken ct);
    Task<bool> SubmitAsync(JunkyardSubmitPayload payload, CancellationToken ct);
  }
}