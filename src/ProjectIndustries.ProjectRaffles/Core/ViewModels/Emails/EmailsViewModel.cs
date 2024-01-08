using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Emails;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Emails
{
  public class EmailsViewModel : PageViewModelBase, IRoutableViewModel
  {
    private readonly IEmailRepository _emailRepository;
    private readonly IToastNotificationManager _toasts;
    private readonly IImapService _imapService;
    private readonly IImapConfigPromptService _imapConfigPromptService;
    private readonly IMasterEmailPromptService _masterEmailPromptService;
    private readonly HeaderEmailsViewModel _header;
    private readonly ReadOnlyObservableCollection<EmailRowViewModel> _emails;

    public EmailsViewModel(IMessageBus messageBus, IEmailRepository emailRepository, IToastNotificationManager toasts,
      IScreen hostScreen, HeaderEmailsViewModel header, IImapService imapService,
      IImapConfigPromptService imapConfigPromptService, IMasterEmailPromptService masterEmailPromptService)
      : base("Emails", messageBus)
    {
      _emailRepository = emailRepository;
      _toasts = toasts;
      _header = header;
      _imapService = imapService;
      _imapConfigPromptService = imapConfigPromptService;
      _masterEmailPromptService = masterEmailPromptService;
      HostScreen = hostScreen;

      var emailsStream = emailRepository.Items.Connect()
        .Transform(i => new EmailRowViewModel(i, emailRepository))
        .Publish()
        .RefCount();

      emailsStream
        .Bind(out _emails)
        .DisposeMany()
        .Subscribe();


      var canCreate = this.WhenAnyValue(_ => _.RawEmails)
        .Select(grp => !string.IsNullOrWhiteSpace(grp));

      CreateCommand = ReactiveCommand.CreateFromTask(CreateEmailsAsync, canCreate);

      var canRemove = emailRepository.Items.CountChanged.Select(c => c > 0);
      RemoveAllEmailsCommand = ReactiveCommand.CreateFromTask(RemoveAllEmailsAsync, canRemove);
    }

    private async Task RemoveAllEmailsAsync(CancellationToken ct)
    {
      await _emailRepository.RemoveAllAsync(ct);
      _toasts.Show(ToastContent.Success("All emails were removed."));
    }

    private async Task CreateEmailsAsync(CancellationToken ct)
    {
      var tokens = RawEmails.Split(new[] {"\n", Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
      var emails = new List<Email>(tokens.Length);
      var failedEmail = new List<string>(tokens.Length);
      foreach (var token in tokens)
      {
        if (!Email.TryParse(token, out var email))
        {
          failedEmail.Add(token);
          continue;
        }

        if (!string.IsNullOrWhiteSpace(email.Password) && !email.IsCatchAll)
        {
          email.ImapConfig = await ResolveImapConfigAsync(email, ct);
        }

        await InitializeMasterEmailIfCatchAllAsync(ct, email);

        emails.Add(email);
      }

      await _emailRepository.SaveAsync(emails, ct);
      if (failedEmail.Any())
      {
        _toasts.Show(ToastContent.Warning($"{failedEmail.Count} failed to create. Please check it and try again"));
        RawEmails = string.Join(Environment.NewLine, failedEmail);
        return;
      }

      _toasts.Show(ToastContent.Success("All emails were created"));
      RawEmails = null;
    }

    private async Task InitializeMasterEmailIfCatchAllAsync(CancellationToken ct, Email email)
    {
      if (email.IsCatchAll)
      {
        Email masterEmail = await _masterEmailPromptService.PromptAsync(ct);
        email.MasterEmail = masterEmail;
      }
    }

    private async Task<ImapConfig> ResolveImapConfigAsync(Email email, CancellationToken ct)
    {
      var cfg = await _imapService.LookupAsync(email.GetDomainName(), ct);
      if (cfg == null)
      {
        cfg = await _imapConfigPromptService.PromptAsync(email, ct);
      }

      return cfg;
    }

    public string UrlPathSegment => nameof(EmailsViewModel);
    public IScreen HostScreen { get; }


    public ReadOnlyObservableCollection<EmailRowViewModel> EmailRows => _emails;

    [Reactive] public string RawEmails { get; set; }

    public ReactiveCommand<Unit, Unit> CreateCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> RemoveAllEmailsCommand { get; private set; }

    protected override ViewModelBase GetHeaderContent() => _header;
  }
}