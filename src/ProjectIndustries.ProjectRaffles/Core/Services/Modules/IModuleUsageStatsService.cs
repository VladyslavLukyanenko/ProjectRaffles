using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Modules
{
  public interface IModuleUsageStatsService
  {
    Task<IList<ModuleUsageStat>> GetStatsAsync(string id, CancellationToken ct = default);
    Task CreateFromTaskAsync(RaffleTask task, CancellationToken ct = default);
  }
}