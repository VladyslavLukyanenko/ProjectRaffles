using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public interface IPendingRaffleTasksManager
  {
    Task SpawnAsync(CancellationToken ct = default);
  }
}