using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SnipesUsaModule
{
    [EnableCaching]
    public interface ISnipesUsaClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<SnipesApiParsed> ParseSnipesApiAsync(CancellationToken ct);

        Task<string> GetSizeIdAsync(SnipesApiParsed parsedApi, string model, string size, string store);

        Task<bool> SubmitAsync(AddressFields addressFields, string email, string sizeGuid, string captcha, CancellationToken ct);
    }
}