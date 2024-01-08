using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
  public interface IRafflesApiClient
  {
    Task<IList<SubmissionStatsEntry>> GetStatsAsync(string licenseKey, CancellationToken ct = default);
  }
}