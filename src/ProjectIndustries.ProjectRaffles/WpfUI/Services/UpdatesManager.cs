using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core;
using ProjectIndustries.ProjectRaffles.Core.Clients;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services
{
  public class UpdatesManager : IUpdatesManager
  {
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(1);

    private readonly IUpdateApiClient _updateApiClient;
    private readonly ILicenseKeyProvider _licenseKeyProvider;

    private readonly BehaviorSubject<Version> _nextVersion =
      new BehaviorSubject<Version>(AppConstants.CurrentAppVersion);

    public UpdatesManager(IUpdateApiClient updateApiClient, ILicenseKeyProvider licenseKeyProvider,
      IToastNotificationManager toasts)
    {
      _updateApiClient = updateApiClient;
      _licenseKeyProvider = licenseKeyProvider;
      AvailableVersion = _nextVersion;

      NextVersionAvailable = AvailableVersion.Select(v => v > AppConstants.CurrentAppVersion);
      NextVersionAvailable
        .DistinctUntilChanged()
        .Subscribe(available =>
      {
        if (available)
        {
          toasts.Show(ToastContent.Success(
            $"An updated version '{_nextVersion.Value}' is available. Would you like to update?"));
        }
      });
    }

    public IObservable<Version> AvailableVersion { get; }
    public IObservable<bool> NextVersionAvailable { get; }

    public async Task<Version> CheckForUpdatesAsync(CancellationToken ct = default)
    {
      var nextVersion =
        await _updateApiClient.GetLatestAvailableVersionAsync(_licenseKeyProvider.CurrentLicenseKey, ct);

      _nextVersion.OnNext(nextVersion);

      return nextVersion;
    }

    public void Spawn()
    {
      Observable.Interval(CheckInterval, RxApp.TaskpoolScheduler)
        .Subscribe(_ =>
        {
          Task.Run(async () =>
          {
            await CheckForUpdatesAsync();
          });
        });
    }
  }
}