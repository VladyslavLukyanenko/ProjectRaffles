using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.FenomModule
{
    [EnableCaching]
    public interface IFenomClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductNameAsync(string raffleurl, CancellationToken ct);
        
        [CacheOutput]
        Task<FenomSizeIdentifiers> GetSizeIdentifiersAsync(string raffleurl, string size, CancellationToken ct);

        Task<string> LoginAsync(Account account, CancellationToken ct);

        Task<bool> SubmitEntryAsync(Account account, FenomSizeIdentifiers size, string raffleurl, string captcha, CancellationToken ct);
    }
}