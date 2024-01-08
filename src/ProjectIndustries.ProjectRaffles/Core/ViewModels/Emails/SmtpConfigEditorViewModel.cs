using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Clients;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Services.Emails;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Emails
{
  public class SmtpConfigEditorViewModel : ViewModelBase
  {
    private readonly ISmtpConfigClient _smtpClient;
    private readonly ISmtpValidationService _smtpValidationService;
    private readonly IToastNotificationManager _toasts;

    public SmtpConfigEditorViewModel(IToastNotificationManager toasts, ISmtpConfigClient smtpClient,
      ISmtpValidationService smtpValidationService)
    {
      _toasts = toasts;
      _smtpClient = smtpClient;
      _smtpValidationService = smtpValidationService;
      var canSave = this.WhenAnyValue(_ => _.SmtpHost)
        .CombineLatest(
          this.WhenAnyValue(_ => _.Port),
          this.WhenAnyValue(_ => _.TargetEmail),
          (host, port, t) => !string.IsNullOrWhiteSpace(host) && port > 0 && t != null);

      SaveCommand = ReactiveCommand.CreateFromTask(SaveSmtpConfigAsync, canSave);
    }

    private async Task<SmtpConfig> SaveSmtpConfigAsync(CancellationToken ct)
    {
      var smtpConfig = new SmtpConfig(SmtpHost, Port);
      bool validSmtpConfig = await _smtpValidationService.IsValidAsync(TargetEmail, smtpConfig, ct);
      if (!validSmtpConfig)
      {
        _toasts.Show(ToastContent.Error("Can't connect to SMTP server using provided email and server config"));
        return null;
      }

      await _smtpClient.AddManualAsync(TargetEmail.GetDomainName(), smtpConfig, ct);
      SmtpConfig = smtpConfig;
      return smtpConfig;
    }

    [Reactive] public SmtpConfig SmtpConfig { get; private set; }
    [Reactive] public Email TargetEmail { get; set; }
    [Reactive] public string SmtpHost { get; set; }
    [Reactive] public int Port { get; set; }

    public ReactiveCommand<Unit, SmtpConfig> SaveCommand { get; set; }

    public void Reset()
    {
      TargetEmail = null;
      SmtpHost = null;
      Port = 0;
      SmtpConfig = null;
    }
  }
}