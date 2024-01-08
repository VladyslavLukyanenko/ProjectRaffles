using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WortherEsModule
{
    public interface IWortherEsClient : IModuleHttpClient
    {
        Task<bool> SubmitAsync(string email, CancellationToken ct);
    }
}