using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Events;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks;

namespace ProjectIndustries.ProjectRaffles.Core.EventHandlers
{
  public class SendDiscordNotificationOnPendingRaffleTaskExpired
    : ApplicationEventHandlerBase<PendingRaffleTaskExpired>
  {
    private readonly IWebHookManager _webHookManager;
    private readonly IPendingRaffleTaskService _pendingRaffleTaskService;

    public SendDiscordNotificationOnPendingRaffleTaskExpired(IWebHookManager webHookManager,
      IPendingRaffleTaskService pendingRaffleTaskService)
    {
      _webHookManager = webHookManager;
      _pendingRaffleTaskService = pendingRaffleTaskService;
    }

    protected override async Task HandleAsync(PendingRaffleTaskExpired @event, CancellationToken ct)
    {
      var task = @event.Task;
      if (!task.IsExpired())
      {
        throw new InvalidOperationException($"Task {task.Email.Value} isn't expired yet");
      }

      _webHookManager.EnqueueWebhook(@event.Task);
      task.Notified();
      await _pendingRaffleTaskService.SaveAsync(task, ct);
    }
  }
}