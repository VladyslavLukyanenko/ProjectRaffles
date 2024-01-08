using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.BSTNModule
{
  public interface IBSTNClient : IModuleHttpClient
  {
    Task<bool> SubmitAsync(BSTNSubmitPayload payload, CancellationToken ct);
  }
}