using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public interface IDiscordSettingsService
  {
    IObservable<DiscordSettings> Settings { get; }
    Task UpdateAsync(DiscordSettings settings, CancellationToken ct = default);
    Task RefreshAsync(CancellationToken ct = default);
  }
}