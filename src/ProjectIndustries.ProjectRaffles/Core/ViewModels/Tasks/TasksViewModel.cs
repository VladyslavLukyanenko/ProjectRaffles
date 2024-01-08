using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Tasks;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Tasks
{
  public class TasksViewModel : PageViewModelBase, IRoutableViewModel
  {
    private readonly ReadOnlyObservableCollection<TaskRowViewModel> _tasks;

    public TasksViewModel(IScreen hostScreen, IMessageBus messageBus, TaskSearchViewModel search,
      ITasksService tasksService, TaskEditViewModel taskEditor, IRaffleTaskExecutor taskExecutor,
      IClipboardService clipboardService, IToastNotificationManager toasts)
      : base("Tasks", messageBus)
    {
      HostScreen = hostScreen;
      Search = search;

      var taskRows = tasksService.Tasks.Connect()
        .ObserveOn(RxApp.TaskpoolScheduler)
        .Throttle(TimeSpan.FromMilliseconds(250))
        .Transform(t => new TaskRowViewModel(t, tasksService, taskExecutor, clipboardService, toasts))
        .Publish()
        .RefCount();

      taskRows
        .ObserveOn(RxApp.MainThreadScheduler)
        .Bind(out _tasks)
        .DisposeMany()
        .Subscribe();

      taskEditor.CloseCommand
        .Subscribe(_ =>
        {
          if (taskEditor.IsCreating)
          {
            toasts.Show(ToastContent.Error("Task creation in progress. Please wait until it finishes"));
            return;
          }

          TaskEditor = null;
        });

      search.CreateCommand = ReactiveCommand.Create(() =>
      {
        taskEditor.Reset();
        TaskEditor = taskEditor;
      });


      taskRows
        .Throttle(TimeSpan.FromMilliseconds(250), RxApp.TaskpoolScheduler)
        .AutoRefresh(_ => _.CanBeRunned)
        .Filter(_ => _.CanBeRunned)
        .Transform(_ => _.CanBeRunned)
        .Distinct()
        .Bind(out var hasNotStartedItems)
        .Subscribe();

      var hasNotStartedTasks = hasNotStartedItems.ObserveCollectionChanges()
        .Select(_ => hasNotStartedItems.Count > 0)
        .ObserveOn(RxApp.MainThreadScheduler);

      taskRows
        .Throttle(TimeSpan.FromMilliseconds(250), RxApp.TaskpoolScheduler)
        .AutoRefresh(_ => _.IsRunning)
        .Filter(_ => _.IsRunning)
        .Transform(_ => _.IsRunning)
        .Distinct()
        .Bind(out var isRunningItems)
        .Subscribe();

      var hasRunningTasks = isRunningItems.ObserveCollectionChanges()
        .Select(_ => isRunningItems.Count > 0)
        .ObserveOn(RxApp.MainThreadScheduler);

      var hasAnyTask = _tasks.ObserveCollectionChanges()
        .Select(_ => _tasks.Count > 0)
        .DistinctUntilChanged()
        .ObserveOn(RxApp.MainThreadScheduler);

      search.StartAllCommand = ReactiveCommand.Create(() => { taskExecutor.ExecuteAsync(_tasks.Select(_ => _.Task)); },
        hasNotStartedTasks);

      search.StopAllCommand = ReactiveCommand.CreateFromTask(taskExecutor.CancelAllTasks, hasRunningTasks);
      search.DeleteAllCommand = ReactiveCommand.CreateFromTask(async () =>
      {
        await taskExecutor.CancelAllTasks();
        tasksService.Clear();
      }, hasAnyTask);
    }

    [Reactive] public TaskEditViewModel TaskEditor { get; private set; }
    public string UrlPathSegment => nameof(TasksViewModel);
    public IScreen HostScreen { get; }
    public TaskSearchViewModel Search { get; }


    public ReadOnlyObservableCollection<TaskRowViewModel> Tasks => _tasks;

    protected override ViewModelBase GetHeaderContent() => Search;
  }
}