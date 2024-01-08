using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SesinkoModule
{
  [EnableCaching]
  public interface ISesinkoClient : IModuleHttpClient
  {
    [CacheOutput]
    Task<SesinkoParsedRaffle> ParseRaffleAsync(string raffleurl, CancellationToken ct);
    Task<string> CraftFormDataAsync(SesinkoSubmitPayload payload);
    Task<bool> SubmitAsync(string formdata, SesinkoSubmitPayload payload, CancellationToken ct);
  }
}