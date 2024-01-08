using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TresBienModule
{
  [EnableCaching]
  public interface ITresBienClient : IModuleHttpClient
  {
    [CacheOutput]
    Task<TresBienProductTags> GetRaffleTags(string raffleurl, CancellationToken ct);
    Task<bool> SubmitAsync(TresBienSubmitPayload payload, CancellationToken ct);
  }
}