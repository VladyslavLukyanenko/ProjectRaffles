using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Modules
{
  public interface IRaffleReleaseService
  {
    IObservable<IList<RaffleTaskSpec>> Specs { get; }
    Task RefreshReleasesAsync(YearMonth yearMonth, CancellationToken ct = default);
  }
}