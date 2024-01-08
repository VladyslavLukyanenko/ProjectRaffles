using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SOTFModule
{
  public interface ISOTFClient : IModuleHttpClient
  {
    Task<string> ParseRaffleAsync(CancellationToken ct);
    Task<bool> SubmitAsync(SOTFSubmitPayload payload, CancellationToken ct);
  }
}