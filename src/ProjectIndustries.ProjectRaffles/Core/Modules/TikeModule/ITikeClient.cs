using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TikeModule
{
    [EnableCaching]
    public interface ITikeClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleUrl, CancellationToken ct);
        
        [CacheOutput]
        Task<string> GetRegionId(string raffleUrl, string region, CancellationToken ct);

        Task<string> GetCaptchaImage(string raffleUrl, CancellationToken ct);

        Task<bool> SubmitAsync(AddressFields address, string email, string raffleUrl, string captcha, string regionId,
            string houseNumber, string size, CancellationToken ct);
    }
}