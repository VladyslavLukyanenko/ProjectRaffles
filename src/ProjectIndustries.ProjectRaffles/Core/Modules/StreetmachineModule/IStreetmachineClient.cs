using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.StreetmachineModule
{
    [EnableCaching]
    public interface IStreetmachineClient : IModuleHttpClient
    {
        //[CacheOutput]
        //Task<string> GetProductAsync(string raffleUrl, CancellationToken ct);
        
        Task<string> LoginAsync(Account account, CancellationToken ct);

        [CacheOutput]
        Task<StreetmachineParsed> ParseSizesAsync(string raffleUrl, string size, CancellationToken ct);

        Task<bool> SubmitAsync(StreetmachineParsed parsed, string captcha, string raffleUrl, CancellationToken ct);
    }
}