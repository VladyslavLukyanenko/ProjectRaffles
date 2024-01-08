using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
  public interface IRaffleReleaseClient
  {
    Task<IList<RaffleTaskSpec>> GetReleasesAsync(YearMonth yearMonth, string licenseKey,
      CancellationToken ct = default);
  }
}