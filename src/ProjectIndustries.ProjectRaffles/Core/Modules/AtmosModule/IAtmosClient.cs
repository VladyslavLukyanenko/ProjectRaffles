using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AtmosModule
{
    [EnableCaching]
    public interface IAtmosClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<AtmosParsed> ParseApiAsync(CancellationToken ct);

        Task<AtmosSelectedProduct> GetSizeAndModelAsync(AtmosParsed parsed, string model, string size, string store);

        Task<bool> SubmitAsync(AddressFields addressFields, AtmosSelectedProduct product, string captcha, string email, string instagram, string useAddress, CancellationToken ct);
    }
}