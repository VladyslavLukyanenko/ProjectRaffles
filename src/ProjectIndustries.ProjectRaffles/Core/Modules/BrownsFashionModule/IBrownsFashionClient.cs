using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.BrownsFashionModule
{
    [EnableCaching]
    public interface IBrownsFashionClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleurl, CancellationToken ct);

        [CacheOutput]
        Task<string> GetRaffleNameAsync(string raffleurl, CancellationToken ct);

        [CacheOutput]
        Task<string> GetCurrencyCodeAsync(string countryCode, CancellationToken ct);

        Task<string> LoginAsync(Account account, CancellationToken ct);

        Task<bool> SubmitAsync(AddressFields addressFields, Account account, string raffleName, string sizeValue, string currencyCode, CancellationToken ct);
    }
}