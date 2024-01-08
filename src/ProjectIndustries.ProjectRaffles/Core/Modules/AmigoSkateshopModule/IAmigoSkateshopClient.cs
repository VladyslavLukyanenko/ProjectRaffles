using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AmigoSkateshopModule
{
    [EnableCaching]
    public interface IAmigoSkateshopClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleurl, CancellationToken ct);
        [CacheOutput]
        Task<AmigoSkateShopParsedForm> ParseFormAsync(string url, CancellationToken ct);
        Task<bool> SubmitAsync(AmigoSkateshopSubmitPayload payload, CancellationToken ct);
    }
}