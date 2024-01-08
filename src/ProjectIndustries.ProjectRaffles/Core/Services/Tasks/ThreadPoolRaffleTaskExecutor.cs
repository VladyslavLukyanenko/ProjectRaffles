using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Events;
using ProjectIndustries.ProjectRaffles.Core.Services.Modules;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Tasks
{
  public class ThreadPoolRaffleTaskExecutor : IRaffleTaskExecutor
  {
    private readonly IMessageBus _messageBus;
    private readonly ILogger<ThreadPoolRaffleTaskExecutor> _logger;
    private readonly IGeneralSettingsService _generalSettingsService;
    private readonly IModuleUsageStatsService _statsService;

    private readonly Dictionary<RaffleTask, CancellationTokenSource> _taskCancellationsDict =
      new Dictionary<RaffleTask, CancellationTokenSource>();

    public ThreadPoolRaffleTaskExecutor(IMessageBus messageBus, ILogger<ThreadPoolRaffleTaskExecutor> logger,
      IGeneralSettingsService generalSettingsService, IModuleUsageStatsService statsService)
    {
      _messageBus = messageBus;
      _logger = logger;
      _generalSettingsService = generalSettingsService;
      _statsService = statsService;
    }


    public async Task ExecuteAsync(RaffleTask task, CancellationToken ct = default)
    {
      if (task.Status.Kind != RaffleStatusKind.Ready)
      {
        return;
      }

      task.Schedule();
      await ExecuteProcessingAsync(task, ct);
    }

    private async Task ExecuteProcessingAsync(RaffleTask task, CancellationToken ct)
    {
      var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
      _taskCancellationsDict[task] = cts;
      await Task.Run(async () =>
      {
        try
        {
          await ProcessAsync(task, cts.Token);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
          task.Cancel();
        }
        catch (RaffleFailedException fexc)
        {
          _logger.LogError(fexc, "Can't execute raffle task");
          task.FailedWithCause(fexc.Message, fexc.RootCause);
        }
        catch (Exception exc)
        {
          _logger.LogError(exc, "Can't execute raffle task");
          task.UnknownError("Unhandled: " + exc.Message);
        }
        finally
        {
          _taskCancellationsDict.Remove(task);
        }
      }, CancellationToken.None);
    }

    public void Cancel(RaffleTask task)
    {
      if (task != null && _taskCancellationsDict.TryGetValue(task, out var cts))
      {
        if (cts.IsCancellationRequested)
        {
          return;
        }

        cts.Cancel(true);
      }
    }

    public async Task CancelAllTasks()
    {
      await Task.Run(async () =>
      {
        var tasks = _taskCancellationsDict.Keys.ToArray().Select(t => Task.Run(() => Cancel(t)));
        await Task.WhenAll(tasks);
      });
    }

    public async Task ExecuteAsync(IEnumerable<RaffleTask> tasks, CancellationToken ct = default)
    {
      await Task.Run(async () =>
      {
        var toSpawn = tasks.Where(t => t.Status.Kind == RaffleStatusKind.Ready)
          .ToArray();
        foreach (var raffleTask in toSpawn)
        {
          raffleTask.Schedule();
        }
        
        var hotTasks = toSpawn.Select(t => ExecuteProcessingAsync(t, ct));
        await Task.WhenAll(hotTasks);
      }, ct);
    }

    private async Task ProcessAsync(RaffleTask task, CancellationToken ct = default)
    {
      ct.ThrowIfCancellationRequested();

      var currentSettings = _generalSettingsService.CurrentSettings;

      var delay = currentSettings.GenerateDelay();
      await task.DelayBeforeProcessing(delay, ct);


      await task.Module.ExecuteAsync(task, ct);
      await _statsService.CreateFromTaskAsync(task, ct);
      _messageBus.SendMessage(new RaffleTaskCompleted(task));
    }
  }
}