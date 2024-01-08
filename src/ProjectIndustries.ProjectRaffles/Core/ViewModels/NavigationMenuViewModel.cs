using ReactiveUI;

using Splat;

using System;
using System.Collections.Generic;
using System.Reactive;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Accounts;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.CustomLists;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Dashboard;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Emails;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Profiles;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Proxies;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels
{
  public class NavigationMenuViewModel
    : ViewModelBase
  {
    public NavigationMenuViewModel(RoutingState routingState)
    {
      NavigationButtons = new List<NavigationButtonItemViewModel>
      {
        new NavigationButtonItemViewModel
        {
          RegularIconSrc = "/Assets/Icons/icon-dashboard.png",
          ActiveIconSrc = "/Assets/Icons/icon-dashboard_active.png",
          Command = ReactiveCommand.Create(SetActiveScreen<DashboardViewModel>)
        },
        new NavigationButtonItemViewModel
        {
          RegularIconSrc = "/Assets/Icons/icon-tasks.png",
          ActiveIconSrc = "/Assets/Icons/icon-tasks_active.png",
          Command = ReactiveCommand.Create(SetActiveScreen<TasksViewModel>)
        },
        new NavigationButtonItemViewModel
        {
          RegularIconSrc = "/Assets/Icons/icon-proxies.png",
          ActiveIconSrc = "/Assets/Icons/icon-proxies_active.png",
          Command = ReactiveCommand.Create(SetActiveScreen<ProxiesViewModel>)
        },
        new NavigationButtonItemViewModel
        {
          RegularIconSrc = "/Assets/Icons/icon-accounts.png",
          ActiveIconSrc = "/Assets/Icons/icon-accounts_active.png",
          Command = ReactiveCommand.Create(SetActiveScreen<AccountsViewModel>)
        },
        new NavigationButtonItemViewModel
        {
          RegularIconSrc = "/Assets/Icons/icon-emails.png",
          ActiveIconSrc = "/Assets/Icons/icon-emails_active.png",
          Command = ReactiveCommand.Create(SetActiveScreen<EmailsViewModel>)
        },
        new NavigationButtonItemViewModel
        {
          RegularIconSrc = "/Assets/Icons/icon-profiles.png",
          ActiveIconSrc = "/Assets/Icons/icon-profiles_active.png",
          Command = ReactiveCommand.Create(SetActiveScreen<ProfilesViewModel>)
        },
        new NavigationButtonItemViewModel
        {
          RegularIconSrc = "/Assets/Icons/icon-custom-lists.png",
          ActiveIconSrc = "/Assets/Icons/icon-custom-lists_active.png",
          Command = ReactiveCommand.Create(SetActiveScreen<CustomListsViewModel>)
        },
        new NavigationButtonItemViewModel
        {
          RegularIconSrc = "/Assets/Icons/icon-settings.png",
          ActiveIconSrc = "/Assets/Icons/icon-settings_active.png",
          Command = ReactiveCommand.Create(SetActiveScreen<SettingsViewModel>)
        },
      };

      Router = routingState;

      foreach (var item in NavigationButtons)
      {
        item.Command.Subscribe(_ =>
        {
          foreach (var i in NavigationButtons)
          {
            i.IsActivated = i == item;
          }
        });
      }

      CheckForUpdatesCommand = ReactiveCommand.Create(() =>
      {
        
      });

    }


    public ReactiveCommand<Unit, Unit> CheckForUpdatesCommand { get; private set; }
    public IList<NavigationButtonItemViewModel> NavigationButtons { get; }
    public RoutingState Router { get; }
    public Version CurrentAppVersion => AppConstants.CurrentAppVersion;

    private void SetActiveScreen<TViewModel>()
      where TViewModel : class, IRoutableViewModel, IPageViewModel
    {
      var viewModel = Locator.Current.GetService<TViewModel>();
      Router.NavigateAndReset.Execute(viewModel).Subscribe();
      viewModel.PageActivated();
    }
  }
}