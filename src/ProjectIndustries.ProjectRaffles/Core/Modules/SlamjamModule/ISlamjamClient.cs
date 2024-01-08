using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SlamjamModule
{
  public interface ISlamjamClient : IModuleHttpClient
  {
    Task<bool> SubmitAsync(SlamjamSubmitPayload payload, CancellationToken ct);
  }
}