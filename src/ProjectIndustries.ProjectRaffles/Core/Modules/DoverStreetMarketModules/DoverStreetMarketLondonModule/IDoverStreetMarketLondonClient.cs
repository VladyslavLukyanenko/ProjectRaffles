using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketLondonModule
{
    [EnableCaching]
    public interface IDoverStreetMarketLondonClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<DoverStreetMarketLondonParsedRaffleFields> ParseRaffleAsync(string raffleurl, string variant, string question, bool containsHiddenFields, CancellationToken ct);
        Task<bool> SubmitAsync(DoverStreetMarketLondonSubmitPayload payload, CancellationToken ct);
    }
}