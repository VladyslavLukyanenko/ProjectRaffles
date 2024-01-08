using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.BlowoutModule
{
  [EnableCaching]
  public interface IBlowoutClient: IModuleHttpClient
  {
    Task<string> GenerateCsrfTokenAsync();
    [CacheOutput]
    Task<BlowoutParsedRaffle> ParseRaffleAsync(string raffleurl, CancellationToken ct);
   // Task<string> GetCaptchaImage(CancellationToken ct);
    Task<bool> SubmitAsync(BlowoutSubmitPayload payload, CancellationToken ct);
  }
}