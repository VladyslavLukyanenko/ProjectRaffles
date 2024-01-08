using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public class PendingRaffleTasksManager : IPendingRaffleTasksManager
  {
    private readonly IPendingRaffleTaskService _pendingRaffleTaskService;
    private readonly IMailsWatchersOrchestrator _orchestrator;
    private bool _isSpawned;

    public PendingRaffleTasksManager(IPendingRaffleTaskService pendingRaffleTaskService,
      IMailsWatchersOrchestrator orchestrator)
    {
      _pendingRaffleTaskService = pendingRaffleTaskService;
      _orchestrator = orchestrator;
    }

    public async Task SpawnAsync(CancellationToken ct = default)
    {
      if (_isSpawned)
      {
        return;
      }

      _isSpawned = true;
      IList<PendingRaffleTask> pending = await _pendingRaffleTaskService.GetPendingAsync(ct);
      _ = Task.Factory.StartNew(() => _orchestrator.SpawnAllPendingTasksAsync(pending, ct), ct,
        TaskCreationOptions.LongRunning, TaskScheduler.Default);

      Observable.Interval(TimeSpan.FromMinutes(10),
          RxApp.TaskpoolScheduler)
        .Subscribe(async _ => { await StopExpiredTasksAsync(ct); });
    }

    private async Task StopExpiredTasksAsync(CancellationToken ct)
    {
      var expired = await _pendingRaffleTaskService.GetExpiredTasksAsync(ct);
      foreach (var pendingRaffleTask in expired)
      {
        _orchestrator.StopTask(pendingRaffleTask);
      }
    }
  }
}