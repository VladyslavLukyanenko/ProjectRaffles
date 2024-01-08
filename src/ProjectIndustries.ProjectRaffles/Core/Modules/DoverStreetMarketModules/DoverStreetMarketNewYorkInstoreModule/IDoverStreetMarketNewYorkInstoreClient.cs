using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketNewYorkInstoreModule
{
    [EnableCaching]
    public interface IDoverStreetMarketNewYorkInstoreClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<DoverStreetMarketNewYorkInstoreParsed> ParseRaffleAsync(string raffleurl, string question, CancellationToken ct);
        Task<bool> SubmitAsync(DoverStreetMarketNewYorkInstorePayload payload, CancellationToken ct);
    }
}