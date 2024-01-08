using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WoodWoodInstoreModule
{
    [EnableCaching]
    public interface IWoodWoodInstoreClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetPhoneCodeAsync(string countryId, CancellationToken ct);
        Task<string> GetIPAsync(CancellationToken ct);
        Task<bool> SubmitAsync(WoodWoodInstorePayload payload, CancellationToken ct);
    }
}