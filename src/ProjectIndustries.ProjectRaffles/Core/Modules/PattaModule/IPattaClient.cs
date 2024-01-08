using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PattaModule
{
    [EnableCaching]
    public interface IPattaClient : IModuleHttpClient
    {
        Task LoginAsync(Account account, CancellationToken ct);
        
        [CacheOutput]
        Task<string> GetProductAsync(string raffleUrl, CancellationToken ct);
        
        [CacheOutput]
        Task<PattaParsed> ParseRaffleAsync(string raffleUrl, string sizeVariant, CancellationToken ct);

        Task<bool> SubmitAsync(AddressFields addressFields, Account account, PattaParsed parsed, string sizeSku, string raffleUrl, string instagramAccount, CancellationToken ct);
    }
}