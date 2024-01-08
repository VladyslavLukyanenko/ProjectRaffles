using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SvdModule
{
  public interface ISvdClient : IModuleHttpClient
  {
    Task<string> GetShippingMethodAsync(Profile profile, Account account, string raffleId, CancellationToken ct);
    Task<string> GetAuthorizationFingerprintAsync(Account account, CancellationToken ct);
    Task<bool> SubmitRaffleAsync(SvdRaffleSubmitPayload payload, CancellationToken ct);
  }
}