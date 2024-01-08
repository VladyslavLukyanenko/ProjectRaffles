using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.UsgStoreModule
{
    [EnableCaching]
    public interface IUsgStoreClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleUrl, CancellationToken ct);

        [CacheOutput]
        Task<UsgStoreParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct);

        Task<bool> SubmitAsync(AddressFields profile, string email, UsgStoreParsed parsed, string size, string raffleurl, string captcha, CancellationToken ct);
    }
}