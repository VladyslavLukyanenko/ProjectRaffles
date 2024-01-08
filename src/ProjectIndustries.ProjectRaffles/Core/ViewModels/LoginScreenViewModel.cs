using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels
{
  public class LoginScreenViewModel : ViewModelBase
  {
    public LoginScreenViewModel(ILicenseKeyProvider licenseKeyProvider, IIdentityService identityService,
      IPreloadService preloadService, ILicenseKeyStore licenseKeyStore, IToastNotificationManager toasts,
      ILogger<LoginScreenViewModel> logger, UpdateViewModel update)
    {
      Update = update;
      logger.LogDebug("Configuring LoginViewModel");
      var isLicenseKeyValid = this.WhenAnyValue(_ => _.LicenseKey)
        .Select(key => !string.IsNullOrEmpty(key));

      var canExecute = new BehaviorSubject<bool>(false);
      LoginCommand = ReactiveCommand.CreateFromTask<Unit>(async (_, ct) =>
      {
        licenseKeyProvider.UseLicenseKey(LicenseKey);
        var authResult = await identityService.FetchIdentityAsync(ct);
        if (authResult?.IsSuccess == true)
        {
          await licenseKeyStore.StoreKeyAsync(LicenseKey, ct);
          await preloadService.PreAuthPreloadAsync(ct);
          identityService.Authenticate(authResult);
          await preloadService.PostAuthPreloadAsync(ct);
        }
        else
        {
          toasts.Show(ToastContent.Error(authResult?.Message ?? "Can't authenticate", "Authentication failed."));
        }
      }, canExecute);
      logger.LogDebug("Created login command");

      logger.LogDebug("Subscribed for check if license if valid");
      isLicenseKeyValid
        .CombineLatest(LoginCommand.IsExecuting, (keyValid, isExecuting) => (keyValid, isExecuting))
        .Select(p => p.keyValid && !p.isExecuting)
        .Subscribe(can => canExecute.OnNext(can));

      update.CheckForUpdatesCommand.IsExecuting.CombineLatest(LoginCommand.IsExecuting,
          (isChecking, isLoginIn) => isChecking || isLoginIn)
        .ToPropertyEx(this, _ => _.IsBusy);

      RestorePreviouslyUsedKeyCommand = ReactiveCommand.CreateFromTask(async ct =>
      {
        logger.LogDebug("Trying to read previous stored key");
        LicenseKey = await licenseKeyStore.GetStoredKeyAsync(ct);
        logger.LogDebug("Previous key restore finished");
      });

      RestorePreviouslyUsedKeyCommand.Execute().Subscribe();

      logger.LogDebug("LoginViewModel is being initialized");
      update.CheckForUpdatesCommand.Execute()
        .Subscribe(isAvailable =>
        {
          if (isAvailable)
          {
            update.PreventCancellation = true;
            update.PrepareToUpdateCommand.Execute().Subscribe();
          }
        });
    }

    public UpdateViewModel Update { get; }

    [Reactive] public string LicenseKey { get; set; }
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public bool IsBusy { [ObservableAsProperty] get; }

    public ReactiveCommand<Unit, Unit> RestorePreviouslyUsedKeyCommand { get; private set; }
  }
}