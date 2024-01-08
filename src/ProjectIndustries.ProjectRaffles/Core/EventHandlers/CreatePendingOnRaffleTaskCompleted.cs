using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Events;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.Core.EventHandlers
{
  public class CreatePendingOnRaffleTaskCompleted : ApplicationEventHandlerBase<RaffleTaskCompleted>
  {
    private readonly IPendingRaffleTaskService _pendingRaffleTaskService;
    private readonly IMailsWatchersOrchestrator _orchestrator;
    private readonly IMessageBus _messageBus;

    public CreatePendingOnRaffleTaskCompleted(IPendingRaffleTaskService pendingRaffleTaskService,
      IMailsWatchersOrchestrator orchestrator, IMessageBus messageBus)
    {
      _pendingRaffleTaskService = pendingRaffleTaskService;
      _orchestrator = orchestrator;
      _messageBus = messageBus;
    }

    protected override async Task HandleAsync(RaffleTaskCompleted @event, CancellationToken ct)
    {
      var pendingTask = PendingRaffleTask.Create(@event.Task);
      await _pendingRaffleTaskService.SaveAsync(pendingTask, ct);
      if (pendingTask.CanBeTracked())
      {
        if (@event.Task.Status.IsSuccessful)
        {
          _ = _orchestrator.SpawnPendingTaskAsync(pendingTask, ct);
        }
      }

      if (pendingTask.IsWinner.HasValue)
      {
        _messageBus.SendMessage(new PendingRaffleTaskResultsReceived(pendingTask));
      }
    }
  }
}