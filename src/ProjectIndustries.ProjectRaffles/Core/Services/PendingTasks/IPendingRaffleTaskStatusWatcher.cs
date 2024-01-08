using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public interface IPendingRaffleTaskStatusWatcher
  {
    Task WatchAsync(PendingRaffleTask pendingTask, CancellationToken ct);
  }
}