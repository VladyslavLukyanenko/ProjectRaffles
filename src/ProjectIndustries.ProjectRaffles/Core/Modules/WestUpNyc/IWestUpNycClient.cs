using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WestUpNyc
{
    [EnableCaching]
    public interface IWestUpNycClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleUrl, CancellationToken ct);

        [CacheOutput]
        Task<WestUpNycParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct);

        Task<bool> SubmitAsync(AddressFields addressFields, WestUpNycParsed parsed, string email,
            string size, CancellationToken ct);
    }
}