using System;
using System.Reactive;
using System.Reactive.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Tasks;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Tasks
{
  public class TaskRowViewModel : ViewModelBase
  {
    public TaskRowViewModel(RaffleTask task, ITasksService tasksService, IRaffleTaskExecutor taskExecutor,
      IClipboardService clipboardService, IToastNotificationManager toasts)
    {
      Task = task;
      ProviderName = task.ProviderName;
      ProductName = task.ProductName;
      Size = task.Size;
      ProfileName = task.Profile?.ProfileName ?? "<No profile>";
      ProxyGroupName = task.ProxyGroupName;

      task.WhenAnyValue(_ => _.ProductName).ToPropertyEx(this, _ => _.ProductName);

      var statusObs = this.WhenAnyValue(_ => _.Status)
        .Where(s => s != null)
        .DistinctUntilChanged()
        .Publish()
        .RefCount();

      statusObs
        .Subscribe(_ => { IsDescVisible = !string.IsNullOrEmpty(_.Description); });

      statusObs
        .Select(_ => _.IsRunning)
        .ToPropertyEx(this, _ => _.IsRunning);

      var canStart = statusObs.Select(status => status.Kind == RaffleStatusKind.Ready);
      var canRemove = statusObs.Select(status => !status.IsRunning);
      var canStop = statusObs.Select(status => status.IsRunning);

      canStart
        .ToPropertyEx(this, _ => _.CanBeRunned);

      canStop
        .ToPropertyEx(this, _ => _.CanBeStopped);

      canRemove
        .ToPropertyEx(this, _ => _.CanBeRemoved);

      StartCommand = ReactiveCommand.Create(() => { taskExecutor.ExecuteAsync(task); }, canStart);
      StopCommand = ReactiveCommand.Create(() => { taskExecutor.Cancel(task); }, canStop);
      RemoveCommand = ReactiveCommand.Create(() => { tasksService.Remove(task); }, canRemove);


      Task.WhenAnyValue(_ => _.Status)
        .Throttle(TimeSpan.FromMilliseconds(300))
        .ObserveOn(RxApp.MainThreadScheduler)
        .ToPropertyEx(this, _ => _.Status, RaffleStatus.Created);

      CopyStatusClipboardCommand = ReactiveCommand.CreateFromTask(async ct =>
      {
        var message = $"{Status.Name}\n{Status.Description}";
        await clipboardService.SetTextAsync(message, ct);
        toasts.Show(ToastContent.Success("Status copied to clipboard"));
      });
    }

    public RaffleTask Task { get; }

    [Reactive] public bool IsDescVisible { get; set; }

    public RaffleStatus Status { [ObservableAsProperty] get; }
    [Reactive] public string ProviderName { get; set; }
    [Reactive] public string Size { get; set; }
    [Reactive] public string ProfileName { get; set; }
    [Reactive] public string ProxyGroupName { get; set; }
    public string ProductName { [ObservableAsProperty] get; }


    public bool IsRunning { [ObservableAsProperty] get; }

    public bool CanBeRunned { [ObservableAsProperty] get; }
    public bool CanBeStopped { [ObservableAsProperty] get; }
    public bool CanBeRemoved { [ObservableAsProperty] get; }


    public ReactiveCommand<Unit, Unit> StartCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> RemoveCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> CopyStatusClipboardCommand { get; private set; }
  }
}