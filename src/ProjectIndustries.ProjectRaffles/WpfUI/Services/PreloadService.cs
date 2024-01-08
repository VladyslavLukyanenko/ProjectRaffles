using System.Threading;
using System.Threading.Tasks;
using Elastic.Apm;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Accounts;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;
using ProjectIndustries.ProjectRaffles.Core.Services.CustomLists;
using ProjectIndustries.ProjectRaffles.Core.Services.Emails;
using ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks;
using ProjectIndustries.ProjectRaffles.Core.Services.Profiles;
using ProjectIndustries.ProjectRaffles.Core.Services.Proxies;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Accounts;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Dashboard;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Emails;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Profiles;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Proxies;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Tasks;
using Splat;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services
{
  public class PreloadService : IPreloadService
  {
    private bool _isPreAuthPreloaded;

    public Task PreAuthPreloadAsync(CancellationToken ct = default)
    {
      if (_isPreAuthPreloaded)
      {
        return Task.CompletedTask;
      }

      _isPreAuthPreloaded = true;

      Resolve<ISecurityManager>().Spawn();
      Resolve<IApplicationEventsManager>().Spawn();
      Resolve<IWebHookManager>().Spawn();
      Resolve<IUpdatesManager>().Spawn();

      return Task.CompletedTask;
    }

    public async Task PostAuthPreloadAsync(CancellationToken ct = default)
    {
      var notCancel = CancellationToken.None;
      Resolve<IToastNotificationManager>().Suspend();
      await Task.WhenAll(
        Resolve<ICountriesService>().InitializeAsync(notCancel),
        Resolve<IProfilesRepository>().InitializeAsync(notCancel),
        Resolve<IEmailRepository>().InitializeAsync(notCancel),
        Resolve<ICustomListRepository>().InitializeAsync(notCancel),
        Resolve<IStatsService>().RefreshAsync(notCancel),
        Resolve<IDiscordSettingsService>().RefreshAsync(notCancel),
        Resolve<ICaptchaRepository>().InitializeAsync(notCancel),
        Resolve<IProxyGroupsRepository>().InitializeAsync(notCancel),
        Resolve<IGeneralSettingsService>().InitializeAsync(notCancel),
        Resolve<IAccountGroupsRepository>().InitializeAsync(notCancel)
      );

      await Resolve<IPendingRaffleTasksManager>().SpawnAsync(notCancel);

      // they're lazy singletons, so let's preload them
      Resolve<DashboardViewModel>();
      Resolve<AccountsViewModel>();
      Resolve<TasksViewModel>();
      Resolve<ProfilesViewModel>();
      Resolve<ProxiesViewModel>();
      Resolve<EmailsViewModel>();
      Resolve<SettingsViewModel>();
      _ = Task.Factory.StartNew(Resolve<IApmAgent>().Flush, notCancel);
    }

    private static T Resolve<T>()
    {
      return Locator.Current.GetService<T>();
    }
  }
}