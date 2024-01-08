using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
  public interface IStatsApiClient
  {
    Task<bool> SubmitStatsAsync(SubmissionStatsEntry stats, string licenseKey, CancellationToken ct = default);
  }
}