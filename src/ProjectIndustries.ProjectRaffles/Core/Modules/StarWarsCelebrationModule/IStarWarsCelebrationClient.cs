using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.StarWarsCelebrationModule
{
    [EnableCaching]
    public interface IStarWarsCelebrationClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleUrl, CancellationToken ct);
        
        [CacheOutput]
        Task<StarWarsCelebrationParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct);

        Task<bool> SubmitAsync(AddressFields address, StarWarsCelebrationParsed parsed, string email,
            string captcha, CancellationToken ct);
    }
}