using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.EvgaModule
{
    [EnableCaching]
    public interface IEvgaClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleurl, CancellationToken ct);
        Task<EvgaParsedFields> ParseProductAsync(string url, CancellationToken ct);
        Task<bool> SubmitAsync(AddressFields profile, Account account, string url, EvgaParsedFields parsedFields, string captcha,
            CancellationToken ct);
    }
}