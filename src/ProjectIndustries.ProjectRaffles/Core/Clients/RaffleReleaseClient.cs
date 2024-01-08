using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
  public class RaffleReleaseClient : ApiClientBase, IRaffleReleaseClient
  {
    public RaffleReleaseClient(ProjectIndustriesApiConfig apiConfig)
      : base(apiConfig)
    {
    }
    
    public async Task<IList<RaffleTaskSpec>> GetReleasesAsync(YearMonth yearMonth, string licenseKey, CancellationToken ct = default)
    {
      var start = yearMonth.OnDayOfMonth(1).AtMidnight().InUtc().ToInstant().ToUnixTimeSeconds();
      var end = yearMonth.OnDayOfMonth(yearMonth.Calendar.GetDaysInMonth(yearMonth.Year, yearMonth.Month))
        .AtMidnight()
        .With(t => LocalTime.Midnight.PlusMilliseconds(-1))
        .InUtc()
        .ToInstant()
        .ToUnixTimeSeconds();

      var relativeUrl = $"raffles/api/releases?from={start}&to={end}";

      return await GetAsync<IList<RaffleTaskSpec>>(relativeUrl, licenseKey, ct);
    }
  }
}