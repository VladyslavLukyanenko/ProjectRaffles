using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public interface IStatsService
  {
    IObservable<IList<SubmissionStatsEntry>> Stats { get; }
    Task RefreshAsync(CancellationToken ct = default);
    Task<bool> SubmitStatsAsync(SubmissionStatsEntry stats, CancellationToken ct = default);
  }
}