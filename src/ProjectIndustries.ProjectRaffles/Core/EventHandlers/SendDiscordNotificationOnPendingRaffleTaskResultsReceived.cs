using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Events;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks;

namespace ProjectIndustries.ProjectRaffles.Core.EventHandlers
{
  public class SendDiscordNotificationOnPendingRaffleTaskResultsReceived
    : ApplicationEventHandlerBase<PendingRaffleTaskResultsReceived>
  {
    private readonly IWebHookManager _webHookManager;
    private readonly IPendingRaffleTaskService _pendingRaffleTaskService;

    public SendDiscordNotificationOnPendingRaffleTaskResultsReceived(IWebHookManager webHookManager,
      IPendingRaffleTaskService pendingRaffleTaskService)
    {
      _webHookManager = webHookManager;
      _pendingRaffleTaskService = pendingRaffleTaskService;
    }

    protected override async Task HandleAsync(PendingRaffleTaskResultsReceived @event, CancellationToken ct)
    {
      var task = @event.Task;
      if (!task.IsWinner.HasValue)
      {
        throw new ArgumentException("Received task without result");
      }

      _webHookManager.EnqueueWebhook(@event.Task);
      task.Notified();
      await _pendingRaffleTaskService.SaveAsync(task, ct);
    }
  }
}