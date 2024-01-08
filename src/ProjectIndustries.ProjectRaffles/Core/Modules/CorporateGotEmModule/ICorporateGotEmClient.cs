using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.CorporateGotEmModule
{
    [EnableCaching]
    public interface ICorporateGotEmClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleUrl, CancellationToken ct);
        
        [CacheOutput]
        Task<CorporateGotEmParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct);

        Task<bool> SubmitAsync(AddressFields addressFields, CorporateGotEmParsed parsed, string email, string size,
            CancellationToken ct);
    }
}