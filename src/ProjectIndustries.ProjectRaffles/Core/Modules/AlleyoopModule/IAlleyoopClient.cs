using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AlleyoopModule
{
    [EnableCaching]
    public interface IAlleyoopClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetProductCode(string raffleUrl, CancellationToken ct);

        Task<bool> SubmitAsync(AlleyoopSubmitPayload payload, CancellationToken ct);
    }
}