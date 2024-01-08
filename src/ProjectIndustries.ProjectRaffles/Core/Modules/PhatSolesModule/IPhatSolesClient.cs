using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PhatSolesModule
{
    [EnableCaching]
    public interface IPhatSolesClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleurl, CancellationToken ct);

        [CacheOutput]
        Task<PhatSolesParsedRaffle> ParseRaffleAsync(string raffleUrl, CancellationToken ct);

        Task<bool> SubmitAsync(AddressFields profile, string email, PhatSolesParsedRaffle parsedRaffle, string captcha,
            string size, string instagram, CancellationToken ct);

    }
}