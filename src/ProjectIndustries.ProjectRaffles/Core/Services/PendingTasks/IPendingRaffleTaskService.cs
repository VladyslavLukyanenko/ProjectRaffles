using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public interface IPendingRaffleTaskService : IRepository<PendingRaffleTask>
  {
    Task<IList<PendingRaffleTask>> GetPendingAsync(CancellationToken ct = default);
    Task<IList<PendingRaffleTask>> GetExpiredTasksAsync(CancellationToken ct = default);
  }
}