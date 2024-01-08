using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TeliaDenmarkModule
{
    [EnableCaching]
    public interface ITeliaDenmarkClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleUrl, CancellationToken ct);

        Task<bool> SubmitAsync(AddressFields addressFields, string email, string raffleUrl, string captcha, CancellationToken ct);
    }
}