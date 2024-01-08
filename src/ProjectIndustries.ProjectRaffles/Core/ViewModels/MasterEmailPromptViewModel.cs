using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Emails;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels
{
  public class MasterEmailPromptViewModel : ViewModelBase
  {
    private readonly IImapConfigPromptService _imapConfigPromptService;
    private readonly IImapService _imapService;
    private readonly IToastNotificationManager _toasts;

    public MasterEmailPromptViewModel(IImapConfigPromptService imapConfigPromptService, IImapService imapService,
      IToastNotificationManager toasts)
    {
      _imapConfigPromptService = imapConfigPromptService;
      _imapService = imapService;
      _toasts = toasts;

      var canCreate = this.WhenAnyValue(_ => _.EmailAddress)
        .CombineLatest(this.WhenAnyValue(_ => _.Password),
          (email, pwd) => !string.IsNullOrWhiteSpace(email) && !string.IsNullOrEmpty(pwd));
      SaveCommand = ReactiveCommand.CreateFromTask(CreateMasterEmailAsync, canCreate);
      DismissCommand = ReactiveCommand.Create(() => { });
    }

    private async Task<Email> CreateMasterEmailAsync(CancellationToken ct)
    {
      if (!Email.TryCreateRegular(EmailAddress, Password, out var email))
      {
        _toasts.Show(ToastContent.Error("Invalid email address provided. Please check again."));
        return null;
      }

      var cfg = await _imapService.LookupAsync(email.GetDomainName(), ct);
      email.ImapConfig = cfg ?? await _imapConfigPromptService.PromptAsync(email, ct);
      if (email.ImapConfig == null)
      {
        _toasts.Show(ToastContent.Error("IMAP config is required for master email. Please try again."));
        return null;
      }

      return email;
    }

    [Reactive] public string EmailAddress { get; set; }
    [Reactive] public string Password { get; set; }

    public ReactiveCommand<Unit, Email> SaveCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> DismissCommand { get; private set; }

    public void Reset()
    {
      EmailAddress = null;
      Password = null;
    }
  }
}