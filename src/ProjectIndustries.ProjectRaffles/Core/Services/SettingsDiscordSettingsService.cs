using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public class SettingsDiscordSettingsService : IDiscordSettingsService
  {
    private const string SettingsName = "DiscordSettings.json";

    private readonly BehaviorSubject<DiscordSettings> _settings =
      new BehaviorSubject<DiscordSettings>(DiscordSettings.Empty);

    private readonly ISettingsService _settingsService;

    public SettingsDiscordSettingsService(ISettingsService settingsService)
    {
      _settingsService = settingsService;
      Settings = _settings;
    }

    public IObservable<DiscordSettings> Settings { get; }

    public async Task RefreshAsync(CancellationToken ct = default)
    {
      var item = await _settingsService.ReadSettingsOrDefaultAsync<DiscordSettings>(SettingsName, ct: ct);
      if (item == null)
      {
        item = new DiscordSettings();
        await _settingsService.WriteSettingsAsync(SettingsName, item, ct);
      }

      _settings.OnNext(item);
    }

    public async Task UpdateAsync(DiscordSettings settings, CancellationToken ct = default)
    {
      await _settingsService.WriteSettingsAsync(SettingsName, settings, ct);
      _settings.OnNext(settings);
    }
  }
}