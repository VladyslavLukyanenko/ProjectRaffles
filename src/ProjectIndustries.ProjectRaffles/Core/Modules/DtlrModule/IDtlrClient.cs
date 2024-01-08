using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DtlrModule
{
    [EnableCaching]
    public interface IDtlrClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleurl, CancellationToken ct);

        [CacheOutput]
        Task<DtlrParsed> ParseRaffleAsync(string raffleurl, CancellationToken ct);

        Task<bool> SubmitAsync(AddressFields addressFields, DtlrParsed parsed, string email, string raffleUrl, string size, string captcha, CancellationToken ct);
    }
}