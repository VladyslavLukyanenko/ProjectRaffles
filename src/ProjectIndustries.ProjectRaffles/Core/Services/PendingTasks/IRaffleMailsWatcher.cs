using System.Collections.Generic;
using System.Threading;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public interface IRaffleMailsWatcher
  {
    IAsyncEnumerable<IncomeMailMessage> WatchAsync(PendingRaffleTask pendingTask, CancellationToken ct);
  }
}