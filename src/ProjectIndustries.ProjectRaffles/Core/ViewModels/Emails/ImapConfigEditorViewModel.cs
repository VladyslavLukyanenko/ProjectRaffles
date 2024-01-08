using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Services.Emails;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Emails
{
  public class ImapConfigEditorViewModel : ViewModelBase
  {
    private readonly IImapService _imapService;
    private readonly IToastNotificationManager _toasts;

    public ImapConfigEditorViewModel(IImapService imapService, IToastNotificationManager toasts)
    {
      _imapService = imapService;
      _toasts = toasts;
      var canSave = this.WhenAnyValue(_ => _.ImapHost)
        .CombineLatest(
          this.WhenAnyValue(_ => _.Port),
          this.WhenAnyValue(_ => _.TargetEmail),
          (host, port, t) => !string.IsNullOrWhiteSpace(host) && port > 0 && t != null);

      SaveCommand = ReactiveCommand.CreateFromTask(SaveImapConfigAsync, canSave);
    }

    private async Task<ImapConfig> SaveImapConfigAsync(CancellationToken ct)
    {
      var imapConfig = new ImapConfig(ImapHost, Port, UseSsl);
      bool validImapConfig = await _imapService.IsValidAsync(TargetEmail, imapConfig, ct);
      if (!validImapConfig)
      {
        _toasts.Show(ToastContent.Error("Can't connect to IMAP server using provided email and server config"));
        return null;
      }

      await _imapService.AddManualAsync(TargetEmail.GetDomainName(), imapConfig, ct);
      ImapConfig = imapConfig;
      return imapConfig;
    }

    [Reactive] public ImapConfig ImapConfig { get; private set; }
    [Reactive] public Email TargetEmail { get; set; }
    [Reactive] public string ImapHost { get; set; }
    [Reactive] public int Port { get; set; }
    [Reactive] public bool UseSsl { get; set; }

    public ReactiveCommand<Unit, ImapConfig> SaveCommand { get; set; }

    public void Reset()
    {
      TargetEmail = null;
      ImapHost = null;
      Port = 0;
      UseSsl = false;
      ImapConfig = null;
    }
  }
}