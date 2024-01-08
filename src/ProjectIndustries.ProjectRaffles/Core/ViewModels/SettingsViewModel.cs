using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ProjectIndustries.ProjectRaffles.Core.Validators;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Captchas;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels
{
  public class SettingsViewModel
    : PageViewModelBase, IRoutableViewModel
  {
    private readonly IDiscordSettingsService _discordSettingsService;
    private readonly DiscordSettingsValidator _discordSettingsValidator;
    private readonly IToastNotificationManager _toasts;
    private readonly IWebHookManager _webHookManager;
    private readonly IGeneralSettingsService _generalSettingsService;

    public SettingsViewModel(IDiscordSettingsService discordSettingsService,
      IScreen hostScreen, DiscordSettingsValidator discordSettingsValidator, IToastNotificationManager toasts,
      IWebHookManager webHookManager, IMessageBus messageBus, IGeneralSettingsService generalSettingsService,
      CaptchasViewModel captchas, AddCaptchaViewModel addCaptcha)
      : base("Settings", messageBus)
    {
      _discordSettingsService = discordSettingsService;
      _discordSettingsValidator = discordSettingsValidator;
      _toasts = toasts;
      _webHookManager = webHookManager;
      _generalSettingsService = generalSettingsService;
      HostScreen = hostScreen;
      Captchas = captchas;

      addCaptcha.CloseCommand.Subscribe(_ => AddCaptcha = null);

      discordSettingsService.Settings.ToPropertyEx(this, _ => _.DiscordSettings);
      this.WhenAnyValue(_ => _.DiscordSettings)
        .Select(_ => _?.WebHook)
        .DistinctUntilChanged()
        .Subscribe(url => DiscordWebHook = url);

      generalSettingsService.Settings.ToPropertyEx(this, _ => _.GeneralSettings);

      CompositeDisposable compositeSubs = null;
      this.WhenAnyValue(_ => _.GeneralSettings)
        .Where(s => s != null)
        .Subscribe(s =>
        {
          compositeSubs?.Dispose();
          compositeSubs = new CompositeDisposable();
          WatchGeneralSettingsChangesAsync(s, compositeSubs);
        });


      this.WhenAnyValue(_ => _.DiscordWebHook)
        .Throttle(TimeSpan.FromMilliseconds(500))
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(async url => await SaveDiscordSettingsAsync(url));

      SendTestWebhookCommand = ReactiveCommand.CreateFromTask(SendTestWebhookAsync);
      AddCaptchaCommand = ReactiveCommand.Create(() => { AddCaptcha = addCaptcha; });
    }

    private void WatchGeneralSettingsChangesAsync(GeneralSettings s, CompositeDisposable compositeSubs)
    {
      MinimumDelayMs = (int) s.MinimumDelay.TotalMilliseconds;
      MaximumDelayMs = (int) s.MaximumDelay.TotalMilliseconds;
      CatchAllEmailMaterializeTemplate = s.CatchAllEmailMaterializeTemplate;
      this.WhenAnyValue(_ => _.MinimumDelayMs)
        .CombineLatest(
          this.WhenAnyValue(_ => _.MaximumDelayMs),
          this.WhenAnyValue(_ => _.CatchAllEmailMaterializeTemplate),
          (minDelay, maxDelay, tmpl) => (minDelay, maxDelay, tmpl))
        .Throttle(TimeSpan.FromMilliseconds(500))
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(async changes =>
        {
          var minDelay = TimeSpan.FromMilliseconds(changes.minDelay);
          var maxDelay = TimeSpan.FromMilliseconds(changes.maxDelay);
          if (s.MaximumDelay == maxDelay && s.MinimumDelay == minDelay
                                         && s.CatchAllEmailMaterializeTemplate == changes.tmpl)
          {
            return;
          }

          s.MaximumDelay = maxDelay;
          s.MinimumDelay = minDelay;
          s.CatchAllEmailMaterializeTemplate = changes.tmpl?.Trim();
          await _generalSettingsService.SaveAsync(s);
        })
        .DisposeWith(compositeSubs);
    }

    private async Task SendTestWebhookAsync()
    {
      ToastContent content;
      if (await _webHookManager.TestWebhook())
      {
        content = ToastContent.Success("Test webhook sent successfully");
      }
      else
      {
        content = ToastContent.Error("Webhook wasn't sent. Please check urls provided");
      }

      _toasts.Show(content);
    }

    private async Task SaveDiscordSettingsAsync(string url)
    {
      if (url == DiscordSettings.WebHook)
      {
        return;
      }

      var result = await _discordSettingsValidator.ValidateAsync(new DiscordSettings {WebHook = url});
      if (!result.IsValid)
      {
        _toasts.Show(ToastContent.Error(result.Errors.Select(_ => _.ErrorMessage).First()));
        return;
      }

      DiscordSettings.WebHook = url;
      await _discordSettingsService.UpdateAsync(DiscordSettings);
      _toasts.Show(ToastContent.Success("Webhooks saved"));
    }

    public GeneralSettings GeneralSettings { [ObservableAsProperty] get; }
    public DiscordSettings DiscordSettings { [ObservableAsProperty] get; }

    [Reactive] public string CatchAllEmailMaterializeTemplate { get; set; }
    [Reactive] public string DiscordWebHook { get; set; }
    [Reactive] public int MinimumDelayMs { get; set; }

    [Reactive] public int MaximumDelayMs { get; set; }

    [Reactive] public AddCaptchaViewModel AddCaptcha { get; private set; }
    // [Reactive] public CaptchaResolverDescriptor SelectedCaptchaProvider { get; set; }
    // [Reactive] public string CaptchaProviderApiKey { get; set; }

    // public IEnumerable<CaptchaResolverDescriptor> CaptchaResolvers { get; }

    public ReactiveCommand<Unit, Unit> SendTestWebhookCommand { get; set; }
    public ReactiveCommand<Unit, Unit> AddCaptchaCommand { get; private set; }


    public string UrlPathSegment => nameof(SettingsViewModel);
    public IScreen HostScreen { get; }
    public CaptchasViewModel Captchas { get; }
  }
}