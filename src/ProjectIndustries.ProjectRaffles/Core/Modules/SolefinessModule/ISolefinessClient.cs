using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SolefinessModule
{
    [EnableCaching]
    public interface ISolefinessClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleurl, CancellationToken ct);

        [CacheOutput]
        Task<SolefinessParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct);

        Task<string> CraftContentAsync(AddressFields profile, CreditCardFields creditcard, SolefinessParsed parsed, string email, string instagram, string size);
        Task<bool> SubmitAsync(SolefinessParsed parsed, string jsonContent, CancellationToken ct);
    }
}