using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SneakersDelightModule
{
    [EnableCaching]
    public interface ISneakersDelightClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleurl, CancellationToken ct);
        
        [CacheOutput]
        Task<SneakersDelightParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct);

        Task<string> LoginAsync(Account account, CancellationToken ct);

        Task<SneakersDelightUserData> GetAccountInformationAsync(AddressFields addressFields, CancellationToken ct);

        Task<bool> SubmitEntryAsync(SneakersDelightParsed parsed, string size, string raffleurl, SneakersDelightUserData accountInfo,
            CancellationToken ct);
    }
}