using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TsumModule
{
  [EnableCaching]
  public interface ITsumClient : IModuleHttpClient
  {
    [CacheOutput]
    Task<string> GetProductAsync(string raffleUrl, CancellationToken ct);

    [CacheOutput]
    Task<TsumProductTags> ParseRaffleAsync(string raffleUrl, CancellationToken ct);

    Task<bool> SubmitAsync(TsumSubmitPayload payload, CancellationToken ct);
  }
}