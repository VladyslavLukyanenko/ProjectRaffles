using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.CopdateModule
{
    [EnableCaching]
    public interface ICopdateClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleurl, CancellationToken ct);
        
        Task<bool> VerifyAccountAsync(Account account, AddressFields profile, CancellationToken ct);
        
        [CacheOutput]
        Task<CopdateParsedRaffle> ParseRaffleAsync(string raffleUrl, CancellationToken ct);

        Task<bool> SubmitAsync(CopdateParsedRaffle parsedRaffle, Account account, AddressFields profile, string size,
            string gender, string raffleurl, CancellationToken ct);
    }
}