using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WoeiModule
{
    [EnableCaching]
    public interface IWoeiClient : IModuleHttpClient
    {
        Task LoginAsync(Account account, CancellationToken ct);
        [CacheOutput]
        Task<string> GetProductAsync(string raffleurl, CancellationToken ct);

        [CacheOutput]
        Task<WoeiParsed> ParseProductAsync(string raffleUrl, CancellationToken ct);

        Task<bool> SubmitAsync(AddressFields profile, WoeiParsed parsed, Account account, string instagram, string size, string gender, CancellationToken ct);
    }
}