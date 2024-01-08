using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketLondonInstoreModule
{
    [EnableCaching]
    public interface IDoverStreetMarketLondonInstoreClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<DoverStreetMarketLondonInstoreParsed> ParseRaffleAsync(string raffleurl, string question, CancellationToken ct);

        Task<bool> SubmitAsync(DoverStreetMarketLondonInstorePayload payload, CancellationToken ct);
    }
}