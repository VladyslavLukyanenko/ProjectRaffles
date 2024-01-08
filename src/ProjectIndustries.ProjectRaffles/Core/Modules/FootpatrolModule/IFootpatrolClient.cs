using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.FootpatrolModule
{
  public interface IFootpatrolClient : IModuleHttpClient
  {
    Task<string> SizeParserAsync(string raffleurl, string size, CancellationToken ct);
    Task<bool> SubmitAsync(FootpatrolSubmitPayload payload, CancellationToken ct);
  }
}