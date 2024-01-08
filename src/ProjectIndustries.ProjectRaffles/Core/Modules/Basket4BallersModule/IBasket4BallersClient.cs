using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.Basket4BallersModule
{
    [EnableCaching]
    public interface IBasket4BallersClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleUrl, CancellationToken ct);
        
        [CacheOutput]
        Task<Basket4BallersParsedRaffle> ParseRaffleAsync(string raffleUrl, CancellationToken ct);
        Task<string> FindSizeAsync(string raffleUrl, string size, CancellationToken ct);
        Task<bool> SubmitAsync(AddressFields profile, Basket4BallersParsedRaffle parsedRaffle, string size,
            string captcha, string raffleurl, string email, string instagram, CancellationToken ct);
    }
}