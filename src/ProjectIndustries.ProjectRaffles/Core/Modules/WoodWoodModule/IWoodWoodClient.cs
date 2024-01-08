using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WoodWoodModule
{
  [EnableCaching]
  public interface IWoodWoodClient : IModuleHttpClient
  {
    Task<WoodWoodProductTags> GetRaffleTagsAsync(string raffleurl, CancellationToken ct);
    
    [CacheOutput]
    Task<string> GetPhoneCodeAsync(string countryId, CancellationToken ct);
    Task<string> GetIPAsync(CancellationToken ct);
    Task<bool> SubmitAsync(WoodWoodSubmitPayload payload, CancellationToken ct);
  }
}