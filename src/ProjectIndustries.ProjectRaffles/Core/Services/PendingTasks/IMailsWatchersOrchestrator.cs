using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public interface IMailsWatchersOrchestrator
  {
    Task SpawnPendingTaskAsync(PendingRaffleTask task, CancellationToken ct = default);
    Task SpawnAllPendingTasksAsync(IEnumerable<PendingRaffleTask> tasks, CancellationToken ct = default);
    void StopAllTasks();
    void StopTask(PendingRaffleTask task);
  }
}