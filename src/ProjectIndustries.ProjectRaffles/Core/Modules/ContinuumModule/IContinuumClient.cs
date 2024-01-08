using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.ContinuumModule
{
    [EnableCaching]
    public interface IContinuumClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleurl, CancellationToken ct);

        [CacheOutput]
        Task<string> GetFormIdAsync(string raffleurl, CancellationToken ct);

        Task<string> CraftJsonContent(AddressFields profile, string size, string email);

        Task<bool> SubmitAsync(string formId, string jsonContent, AddressFields profile, string email, CancellationToken ct);
    }
}