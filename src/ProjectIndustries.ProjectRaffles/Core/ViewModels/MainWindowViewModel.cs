using System;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Diagnostics;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels
{
  public class MainWindowViewModel : ViewModelBase, IScreen
  {
    public MainWindowViewModel(NavigationMenuViewModel navigationMenu, RoutingState router,
      MainHeaderViewModel mainHeader, UpdateViewModel update, IToastNotificationManager toasts,
      MemoryDumpCreatorViewModel memoryDumpCreator)
    {
      NavigationMenu = navigationMenu;
      Router = router;
      MainHeader = mainHeader;
      Update = update;
      MemoryDumpCreator = memoryDumpCreator;
      navigationMenu.CheckForUpdatesCommand.Subscribe(_ =>
      {
        update.CheckForUpdatesCommand.Execute()
          .Subscribe(isUpdateAvailable =>
          {
            if (!isUpdateAvailable)
            {
              toasts.Show(ToastContent.Information("No updates available. You're using the latest version."));
            }
          });
      });
    }

    public NavigationMenuViewModel NavigationMenu { get; }
    public RoutingState Router { get; }
    public MainHeaderViewModel MainHeader { get; }
    public UpdateViewModel Update { get; }
    public MemoryDumpCreatorViewModel MemoryDumpCreator { get; }
  }
}