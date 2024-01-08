using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels
{
  public class MainHeaderViewModel : ViewModelBase
  {
    public MainHeaderViewModel(IMessageBus messageBus, IIdentityService identityService,
      INotificationsService notificationsService)
    {
      messageBus.Listen<ChangeHeaderTitle>()
        .Select(m => m?.Title)
        .ToPropertyEx(this, _ => _.Title);

      messageBus.Listen<ChangeHeaderContent>()
        .Select(_ => _?.Content)
        .ObserveOn(RxApp.MainThreadScheduler)
        .ToPropertyEx(this, _ => _.Content);

      identityService.User
        .ObserveOn(RxApp.MainThreadScheduler)
        .ToPropertyEx(this, _ => _.User);
      notificationsService.Notifications
        .Throttle(TimeSpan.FromMilliseconds(300))
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(list => Notifications = list);

      ClearCommand = ReactiveCommand.Create(notificationsService.Clear);

      LogOutCommand = ReactiveCommand.Create(identityService.LogOut);
      DeactivateCommand = ReactiveCommand.CreateFromTask(async ct => { await identityService.DeactivateAsync(ct); });
    }

    [Reactive] public IEnumerable<NotificationViewModel> Notifications { get; private set; }
    public User User { [ObservableAsProperty] get; }
    public string Title { [ObservableAsProperty] get; }
    public ViewModelBase Content { [ObservableAsProperty] get; }

    public ReactiveCommand<Unit, Unit> ClearCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> LogOutCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> DeactivateCommand { get; private set; }
  }
}