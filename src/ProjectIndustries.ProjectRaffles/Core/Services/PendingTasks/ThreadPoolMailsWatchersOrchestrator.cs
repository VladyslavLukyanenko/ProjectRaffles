using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Events;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public class ThreadPoolMailsWatchersOrchestrator : IMailsWatchersOrchestrator
  {
    private readonly IMessageBus _messageBus;
    private readonly ILogger<ThreadPoolMailsWatchersOrchestrator> _logger;
    private readonly IPendingRaffleTaskStatusWatcherFactory _watcherFactory;

    private readonly Dictionary<PendingRaffleTask, CancellationTokenSource> _taskCancellationsDict =
      new Dictionary<PendingRaffleTask, CancellationTokenSource>();

    public ThreadPoolMailsWatchersOrchestrator(IMessageBus messageBus,
      ILogger<ThreadPoolMailsWatchersOrchestrator> logger, IPendingRaffleTaskStatusWatcherFactory watcherFactory)
    {
      _messageBus = messageBus;
      _logger = logger;
      _watcherFactory = watcherFactory;
    }

    public async Task SpawnPendingTaskAsync(PendingRaffleTask task, CancellationToken ct = default)
    {
      if (!task.CanBeTracked())
      {
        return;
      }

      if (_taskCancellationsDict.ContainsKey(task))
      {
        throw new InvalidOperationException($"Task '{task.Email.Value}' already spawned");
      }

      if (task.IsWinner.HasValue)
      {
        throw new InvalidOperationException($"Task '{task.Email.Value}' already finished");
      }

      var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
      _taskCancellationsDict[task] = cts;
      await Task.Factory.StartNew(async () =>
      {
        var watcher = _watcherFactory.Create();
        try
        {
          await watcher.WatchAsync(task, cts.Token);

          // todo: handle task expiration
          if (task.IsWinner.HasValue)
          {
            _messageBus.SendMessage(new PendingRaffleTaskResultsReceived(task));
          }
          else if (task.IsExpired())
          {
            _messageBus.SendMessage(new PendingRaffleTaskExpired(task));
          }
        }
        catch (OperationCanceledException)
        {
          // expected
          if (task.IsExpired())
          {
            _messageBus.SendMessage(new PendingRaffleTaskExpired(task));
          }
        }
        catch (Exception exc)
        {
          _logger.LogError(exc, "Can't spawn pending raffle task");
        }
        finally
        {
          _taskCancellationsDict.Remove(task);
        }
      }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public Task SpawnAllPendingTasksAsync(IEnumerable<PendingRaffleTask> tasks, CancellationToken ct = default)
    {
      var hotTasks = tasks.Select(t => SpawnPendingTaskAsync(t, ct));
      return Task.WhenAll(hotTasks);
    }

    public void StopAllTasks()
    {
      foreach (var cts in _taskCancellationsDict.Values.ToArray())
      {
        cts.Cancel(true);
      }
    }

    public void StopTask(PendingRaffleTask task)
    {
      if (_taskCancellationsDict.TryGetValue(task, out var cts))
      {
        cts.Cancel(true);
      }
    }
  }
}