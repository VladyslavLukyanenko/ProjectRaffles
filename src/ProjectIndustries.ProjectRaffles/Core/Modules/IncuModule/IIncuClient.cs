using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.IncuModule
{
    [EnableCaching]
    public interface IIncuClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string url, CancellationToken ct);
        [CacheOutput]
        Task<IncuParsedRaffle> ParseRaffleAsync(string url, CancellationToken ct);
        Task<string> CreateUrlAsync(AddressFields profile, string email, IncuParsedRaffle parsed, string captcha, string size);
        Task<bool> SubmitAsync(string createdUrl, CancellationToken ct);
    }
}