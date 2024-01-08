using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.MahaModule
{
    [EnableCaching]
    public interface IMahaClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> FetchProductAsync(string raffleUrl, CancellationToken ct);
        [CacheOutput]
        Task<MahaParsedRaffle> ParseRaffleAsync(string raffleUrl, CancellationToken ct);
        Task<bool> SubmitAsync(MahaSubmitPayload payload, CancellationToken ct);
    }
}