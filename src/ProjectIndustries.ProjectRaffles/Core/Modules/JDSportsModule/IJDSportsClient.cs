using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.JDSportsModule
{
  public interface IJDSportsClient : IModuleHttpClient
  {
    Task<string> GetBaseUrlAsync(string raffleurl);
    Task<string> GetStoreCountryAsync(string baseurl);
    Task<string> SizeParserAsync(string raffleurl, string size, CancellationToken ct);
    Task<string> SolveCaptchaAsync(string raffleurl, string baseurl, CancellationToken ct);
    Task<bool> SubmitAsync(JDSportsSubmitPayload payload, CancellationToken ct);
  }
}