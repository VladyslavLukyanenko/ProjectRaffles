using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SuccezzModule
{
    [EnableCaching]
    public interface ISuccezzClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleurl, CancellationToken ct);

        [CacheOutput]
        Task<string> GetFormIdAsync(string raffleUrl, CancellationToken ct);

        Task<string> CraftJsonContent(AddressFields profile, string email, string size);

        Task<bool> SubmitAsync(string formId, string jsonContent, CancellationToken ct);
    }
}