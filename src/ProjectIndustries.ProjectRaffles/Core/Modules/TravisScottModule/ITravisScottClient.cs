using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TravisScottModule
{
    [EnableCaching]
    public interface ITravisScottClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<TravisScottParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct);
        Task<bool> SubmitAsync(string raffleUrl, string email, AddressFields address, TravisScottParsed parsed, string size, CancellationToken ct);
    }
}