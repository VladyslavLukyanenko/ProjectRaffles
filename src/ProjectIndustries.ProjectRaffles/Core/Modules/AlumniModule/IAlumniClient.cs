using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AlumniModule
{
  [EnableCaching]
  public interface IAlumniClient : IModuleHttpClient
  {
    [CacheOutput]
    Task<AlumniParsedRaffle> ParseRaffleAsync(CancellationToken ct);
    Task<bool> SubmitAsync(AlumniSubmitPayload payload, CancellationToken ct);
  }
}