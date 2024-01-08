using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.ShinzoParisModule
{
    [EnableCaching]
    public interface IShinzoParisClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleurl, CancellationToken ct);
        [CacheOutput]
        Task<ShinzoParisParsedRaffle> ParseRaffleAsync(string raffleUrl, CancellationToken ct);
        Task<bool> SubmitAsync(ShinzoParisSubmitPayload payload, CancellationToken ct);

    }
}