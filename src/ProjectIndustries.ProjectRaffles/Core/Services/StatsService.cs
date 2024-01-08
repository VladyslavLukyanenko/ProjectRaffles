using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Clients;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public class StatsService : IStatsService
  {
    private readonly IRafflesApiClient _rafflesApiClient;
    private readonly ILicenseKeyProvider _licenseKeyProvider;

    private readonly BehaviorSubject<IList<SubmissionStatsEntry>> _stats =
      new BehaviorSubject<IList<SubmissionStatsEntry>>(Array.Empty<SubmissionStatsEntry>());

    private readonly IStatsApiClient _statsApiClient;
    private readonly Subject<Unit> _invalidationToken = new Subject<Unit>();

    public StatsService(IRafflesApiClient rafflesApiClient, ILicenseKeyProvider licenseKeyProvider,
      IStatsApiClient statsApiClient)
    {
      _rafflesApiClient = rafflesApiClient;
      _licenseKeyProvider = licenseKeyProvider;
      _statsApiClient = statsApiClient;
      Stats = _stats;

      _invalidationToken
        .Throttle(TimeSpan.FromMilliseconds(500))
        .Subscribe(async _ =>
        {
          if (!_stats.HasObservers)
          {
            return;
          }

          await RefreshAsync();
        });
    }

    public IObservable<IList<SubmissionStatsEntry>> Stats { get; }

    public async Task RefreshAsync(CancellationToken ct = default)
    {
      var stats = await _rafflesApiClient.GetStatsAsync(_licenseKeyProvider.CurrentLicenseKey, ct);
      _stats.OnNext(stats);
    }

    public async Task<bool> SubmitStatsAsync(SubmissionStatsEntry stats, CancellationToken ct = default)
    {
      var isSucceeded = await _statsApiClient.SubmitStatsAsync(stats, _licenseKeyProvider.CurrentLicenseKey, ct);
      if (isSucceeded)
      {
        _invalidationToken.OnNext(Unit.Default);
      }

      return isSucceeded;
    }
  }
}