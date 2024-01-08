using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule
{
  public interface IStayRootedClient : IModuleHttpClient
  {
    Task<string> GetCustomerIdAsync(StayRootedValidationPayload payload, CancellationToken ct);
    Task<string> StartEntryProcessAsync(StayRootedEntryPayload payload, CancellationToken ct);
    Task<bool> SubmitAsync(StayRootedFinalPayload payload, CancellationToken ct);
  }
}