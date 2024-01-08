using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Clients;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public class IdentityService : IIdentityService
  {
    private readonly ILicenseKeyApiClient _licenseKeyApiClient;
    private readonly IDeviceInfoProvider _deviceInfoProvider;
    private readonly ILicenseKeyProvider _licenseKeyProvider;
    private readonly ISoftwareInfoProvider _softwareInfoProvider;
    private readonly BehaviorSubject<User> _user = new BehaviorSubject<User>(null);

    public IdentityService(ILicenseKeyApiClient licenseKeyApiClient, IDeviceInfoProvider deviceInfoProvider,
      ILicenseKeyProvider licenseKeyProvider, ISoftwareInfoProvider softwareInfoProvider)
    {
      _licenseKeyApiClient = licenseKeyApiClient;
      _deviceInfoProvider = deviceInfoProvider;
      _licenseKeyProvider = licenseKeyProvider;
      _softwareInfoProvider = softwareInfoProvider;

      User = _user
        .ObserveOn(RxApp.MainThreadScheduler);

      IsAuthenticated = User.Select(user => user != null);
    }

    public User CurrentUser => _user.Value;
    public IObservable<User> User { get; }
    public IObservable<bool> IsAuthenticated { get; }

    public async Task<AuthenticationResult> TryAuthenticateAsync(CancellationToken ct = default)
    {
      var result = await FetchIdentityAsync(ct);
      if (result.IsSuccess)
      {
        _softwareInfoProvider.SetSoftwareVersion(result.SoftwareVersion);
        var expiryDate = result.Expiry.HasValue
          ? (DateTimeOffset?) DateTimeOffset.FromUnixTimeSeconds(result.Expiry.Value)
          : null;

        var user = new User(result.DiscordId, result.UserName, result.Discriminator, expiryDate, result.Avatar);
        _user.OnNext(user);
      }
      else
      {
        Invalidate();
      }

      return result;
    }

    public async Task<AuthenticationResult> FetchIdentityAsync(CancellationToken ct = default)
    {
      var licenseKey = _licenseKeyProvider.CurrentLicenseKey;
      if (string.IsNullOrEmpty(licenseKey))
      {
        return AuthenticationResult.CreateUnkownError();
      }

      var hwid = await _deviceInfoProvider.GetHwidAsync(ct);
      return await _licenseKeyApiClient.Authenticate(licenseKey, hwid, ct);
    }

    public void Authenticate(AuthenticationResult result)
    {
      if (result.IsSuccess)
      {
        _softwareInfoProvider.SetSoftwareVersion(result.SoftwareVersion);
        var expiryDate = result.Expiry.HasValue
          ? (DateTimeOffset?) DateTimeOffset.FromUnixTimeSeconds(result.Expiry.Value)
          : null;

        var user = new User(result.DiscordId, result.UserName, result.Discriminator, expiryDate, result.Avatar);
        _user.OnNext(user);
      }
      else
      {
        Invalidate();
      }
    }

    public void LogOut()
    {
      Invalidate();
    }

    public async Task DeactivateAsync(CancellationToken ct = default)
    {
      await _licenseKeyApiClient.DeactivateOnCurrentDeviceAsync(_licenseKeyProvider.CurrentLicenseKey, ct);
      LogOut();
    }

    private void Invalidate()
    {
      _licenseKeyProvider.Invalidate();
      _user.OnNext(null);
    }
  }
}