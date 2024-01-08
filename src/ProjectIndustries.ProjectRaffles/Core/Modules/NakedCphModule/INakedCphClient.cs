using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NakedCphModule
{
  [EnableCaching]
  public interface INakedCphClient : IModuleHttpClient
  {
    [CacheOutput]
    Task<string> GetProductAsync(string raffleurl, CancellationToken ct);
    Task<string> GetIPAsync();
    [CacheOutput]
    Task<NakedCphProductTags> GetRaffleTags(string raffleurl, CancellationToken ct);
    Task<bool> SubmitAsync(NakedCphSubmitPayload payload, CancellationToken ct);
  }
}