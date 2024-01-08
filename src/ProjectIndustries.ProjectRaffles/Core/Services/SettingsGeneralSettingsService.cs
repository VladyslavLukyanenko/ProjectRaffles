using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public class SettingsGeneralSettingsService : IGeneralSettingsService
  {
    private readonly ISettingsService _settingsService;
    private const string SettingsFileName = "GeneralSettings.json";
    private readonly BehaviorSubject<GeneralSettings> _settings = new BehaviorSubject<GeneralSettings>(null);

    public SettingsGeneralSettingsService(ISettingsService settingsService)
    {
      _settingsService = settingsService;
      Settings = _settings.AsObservable();
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
      var settings =
        await _settingsService.ReadSettingsOrDefaultAsync(SettingsFileName, () => new GeneralSettings(), ct);
      _settings.OnNext(settings);
    }

    public IObservable<GeneralSettings> Settings { get; }
    public GeneralSettings CurrentSettings => _settings.Value;

    public async Task SaveAsync(GeneralSettings settings, CancellationToken ct = default)
    {
      await _settingsService.WriteSettingsAsync(SettingsFileName, _settings.Value, ct);
      await InitializeAsync(ct);
    }
  }
}