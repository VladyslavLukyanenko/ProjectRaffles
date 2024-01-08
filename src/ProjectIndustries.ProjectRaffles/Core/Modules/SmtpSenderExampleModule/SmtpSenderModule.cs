using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Clients;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Emails;
using ProjectIndustries.ProjectRaffles.Core.Services.Templates;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SmtpSenderExampleModule
{
  [RaffleModuleName("Smtp Sender")]
  [RaffleModuleVersion(0, 0, 1)]
  [NoProfileRequires]
  [RaffleModuleType(RaffleModuleType.Custom)]
  public class SmtpSenderModule : RaffleModuleBase
  {
    private static readonly SemaphoreSlim UILock = new SemaphoreSlim(1, 1);

    private static readonly ConcurrentDictionary<Email, SemaphoreSlim> Gates =
      new ConcurrentDictionary<Email, SemaphoreSlim>();

    private readonly EmailPickerField _sender = new EmailPickerField(displayName: "Sender");
    private readonly Field<string> _receiver = new TextField(displayName: "Receiver");
    private readonly Field<string> _subject = new TextField(displayName: "Subject");
    private readonly Field<string> _message = new TemplatedMultilineField(displayName: "Message");

    private readonly ISmtpClient _smtpClient;
    private readonly ITemplateExpandService _expandService;
    private readonly IRaffleModuleExpandContextFactory _expandContextFactory;

    public SmtpSenderModule(ISmtpClient smtpClient, ITemplateExpandService expandService,
      IRaffleModuleExpandContextFactory expandContextFactory)
    {
      _smtpClient = smtpClient;
      _expandService = expandService;
      _expandContextFactory = expandContextFactory;
    }


    protected override IModuleHttpClient HttpClient => null;

    public override IEnumerable<Field> AdditionalFields
    {
      get
      {
        yield return _sender;
        yield return _receiver;
        yield return _subject;
        yield return _message;
      }
    }

    public override async Task InitializeAsync(IRaffleExecutionContext context, CancellationToken ct)
    {
      await base.InitializeAsync(context, ct);
      var expandContext = _expandContextFactory.Create(this, context);
      foreach (var strField in AdditionalFields.OfType<Field<string>>())
      {
        strField.Value = _expandService.Expand(strField, expandContext);
      }
    }

    protected override async Task ExecuteAsync(Profile profile, CancellationToken ct)
    {
      Email email = _sender;
      if (email.IsCatchAll)
      {
        Status = RaffleStatus.CatchAllNotSupported;
        return;
      }

      Status = RaffleStatus.ResolvingSMTPConfig;
      var gate = Gates.GetOrAdd(email, _ => new SemaphoreSlim(1, 1));
      try
      {
        await gate.WaitAsync(ct);
        var dependencyResolver = ExecutionContext.DependencyResolver;
        var smtpValidationService = dependencyResolver.GetService<ISmtpValidationService>();
        var smtpConfigPromptService = dependencyResolver.GetService<ISmtpConfigPromptService>();
        var repository = dependencyResolver.GetService<IEmailRepository>();
        var smtpConfigClient = dependencyResolver.GetService<ISmtpConfigClient>();
        if (email.SmtpConfig == null || !await smtpValidationService.IsValidAsync(email, email.SmtpConfig, ct))
        {
          var cfg = await smtpConfigClient.LookupAsync(email.GetDomainName(), ct);
          if (cfg == null)
          {
            try
            {
              await UILock.WaitAsync(CancellationToken.None);
              cfg = await smtpConfigPromptService.PromptAsync(email, ct);
            }
            finally
            {
              UILock.Release();
            }
          }

          if (cfg == null)
          {
            Status = RaffleStatus.CantGetSMTPConfig;
            throw new OperationCanceledException(Status.Description);
          }

          email.SmtpConfig = cfg;
          await repository.SaveSilentlyAsync(email, ct);
        }
      }
      finally
      {
        if (!ct.IsCancellationRequested)
        {
          gate.Release();
        }

        Gates.TryRemove(email, out _);
      }


      Status = RaffleStatus.Submitting;
      try
      {
        await _smtpClient.SendAsync(_receiver, _subject, _message, _sender, ct);
        Status = RaffleStatus.Succeeded;
      }
      catch (Exception exc)
      {
        Status = RaffleStatus.FailedWithCause("Failed to submit", exc.GetBaseException().Message);
      }
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product {Name = nameof(SmtpSenderModule)});
    }

    public override void SetHttpClientBuilder(IHttpClientBuilder builder)
    {
    }
  }
}