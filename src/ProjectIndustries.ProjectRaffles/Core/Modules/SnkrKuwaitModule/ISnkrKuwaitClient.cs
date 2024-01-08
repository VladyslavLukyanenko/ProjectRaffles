using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SnkrKuwaitModule
{
    [EnableCaching]
    public interface ISnkrKuwaitClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetRaffleProduct(string url, CancellationToken ct);

        [CacheOutput]
        Task<SnkrKuwaitParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct);

        Task<bool> SubmitAsync(AddressFields addressFields, SnkrKuwaitParsed parsed, Account account, string size,
            string raffleUrl,
            CancellationToken ct);
    }
}