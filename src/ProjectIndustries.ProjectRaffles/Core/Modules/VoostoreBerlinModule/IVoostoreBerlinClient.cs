using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.VoostoreBerlinModule
{
  public interface IVoostoreBerlinClient : IModuleHttpClient
  {
    Task<VoostoreBerlinRaffleTags> GetRaffleTagsAsync(string raffleurl, CancellationToken ct);
    Task<bool> SubmitAsync(VoostoreBerlinSubmitPayload payload, CancellationToken ct);
  }
}