using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public class PendingRaffleTaskStatusWatcher : IPendingRaffleTaskStatusWatcher
  {
    private readonly IRaffleMailsWatcher _watcher;
    private readonly IMailRaffleStatusExtractorProvider _extractorProvider;
    private readonly IPendingRaffleTaskService _taskService;
    private readonly IMailRaffleConfirmationHandlerProvider _confirmationHandlerProvider;

    public PendingRaffleTaskStatusWatcher(IRaffleMailsWatcher watcher,
      IMailRaffleStatusExtractorProvider extractorProvider, IPendingRaffleTaskService taskService,
      IMailRaffleConfirmationHandlerProvider confirmationHandlerProvider)
    {
      _watcher = watcher;
      _extractorProvider = extractorProvider;
      _taskService = taskService;
      _confirmationHandlerProvider = confirmationHandlerProvider;
    }

    public async Task WatchAsync(PendingRaffleTask pendingTask, CancellationToken ct)
    {
      IMailRaffleStatusExtractor raffleStatusExtractor = _extractorProvider.Get(pendingTask.ProviderName);
      IMailRaffleConfirmationHandler confirmationHandler = _confirmationHandlerProvider.Get(pendingTask.ProviderName);
      await foreach (var message in _watcher.WatchAsync(pendingTask, ct))
      {
        if (!pendingTask.IsConfirmed() && (confirmationHandler?.IsExpectedMail(message) ?? false))
        {
          await confirmationHandler.ConfirmAsync(message, ct);
          pendingTask.Confirmed();
          await _taskService.SaveAsync(pendingTask, ct);
          continue;
        }

        if (raffleStatusExtractor.IsExpectedMail(message))
        {
          pendingTask.AssignRaffleStatus(raffleStatusExtractor.IsWinner(message));
          await _taskService.SaveAsync(pendingTask, ct);
          break;
        }
      }
    }
  }
}