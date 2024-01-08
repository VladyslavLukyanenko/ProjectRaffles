using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using ProjectIndustries.ProjectRaffles.Core.Clients;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Modules
{
  public class RaffleReleaseService : IRaffleReleaseService
  {
    private readonly BehaviorSubject<IList<RaffleTaskSpec>> _specs =
      new BehaviorSubject<IList<RaffleTaskSpec>>(Array.Empty<RaffleTaskSpec>());

    private readonly IRaffleReleaseClient _client;
    private readonly ILicenseKeyProvider _licenseKeyProvider;

    public RaffleReleaseService(IRaffleReleaseClient client, ILicenseKeyProvider licenseKeyProvider)
    {
      _client = client;
      _licenseKeyProvider = licenseKeyProvider;
      Specs = _specs.AsObservable();
    }

    public IObservable<IList<RaffleTaskSpec>> Specs { get; }

    public async Task RefreshReleasesAsync(YearMonth yearMonth, CancellationToken ct = default)
    {
      var list = await _client.GetReleasesAsync(yearMonth, _licenseKeyProvider.CurrentLicenseKey, ct);
      _specs.OnNext(list);
    }
  }
}