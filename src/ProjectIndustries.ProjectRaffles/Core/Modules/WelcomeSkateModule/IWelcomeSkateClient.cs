using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WelcomeSkateModule
{
    [EnableCaching]
    public interface IWelcomeSkateClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductAsync(string raffleUrl, CancellationToken ct);

        [CacheOutput]
        Task<WelcomeSkateParsed> GetIdxValuesAsync(string raffleUrl, CancellationToken ct);

        Task<bool> SubmitAsync(AddressFields addressFields, WelcomeSkateParsed parsed, string email, string captcha,
            string size, CancellationToken ct);
    }
}