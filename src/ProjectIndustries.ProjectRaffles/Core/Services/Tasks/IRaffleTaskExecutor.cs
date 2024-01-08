using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Tasks
{
  public interface IRaffleTaskExecutor
  {
    Task ExecuteAsync(RaffleTask task, CancellationToken ct = default);
    void Cancel(RaffleTask task);
    Task CancelAllTasks();
    Task ExecuteAsync(IEnumerable<RaffleTask> tasks, CancellationToken ct = default);
  }
}