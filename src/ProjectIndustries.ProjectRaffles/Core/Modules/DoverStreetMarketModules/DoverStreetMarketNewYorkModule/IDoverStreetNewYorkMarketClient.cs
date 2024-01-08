using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketNewYorkModule
{
    [EnableCaching]
    public interface IDoverStreetMarketNewYorkClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<DoverStreetMarketNewYorkParsedRaffleFields> ParseRaffleAsync(string raffleurl, string variant, string parsetype, string question, CancellationToken ct);
        Task<bool> SubmitAsync(DoverStreetMarketNewYorkSubmitPayload payload, CancellationToken ct);
    }
}