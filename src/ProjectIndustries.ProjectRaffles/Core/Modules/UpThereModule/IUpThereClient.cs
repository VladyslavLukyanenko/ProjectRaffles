using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.UpThereModule
{
  [EnableCaching]
  public interface IUpThereClient : IModuleHttpClient
  {
    [CacheOutput]
    Task<string> GetProductAsync(string raffleUrl, CancellationToken ct);

    Task<string> LoginAsync(Account account, CancellationToken ct);
    Task<string> GetRaffleEndpointAsync(string raffleUrl, CancellationToken ct);
    Task<UpThereProduct> GetProductAsync(AddressFields addressFields, string raffleEndpoint, CancellationToken ct);

    Task<bool> SubmitAsync(AddressFields addressFields, CreditCardFields creditCardFields, Account account,
      UpThereProduct product, string raffleEndpoint, string size, CancellationToken ct);
  }
}