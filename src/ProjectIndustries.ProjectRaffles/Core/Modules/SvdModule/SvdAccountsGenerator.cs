using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators;
using ProjectIndustries.ProjectRaffles.Core.Services.Accounts;
using ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SvdModule
{
  public class SvdAccountsGenerator : ProfileDependentAccountGeneratorBase
  {
    private const int PasswordLen = 8;
    // private readonly EmailPickerField _email = new EmailPickerField(displayName: "Email");

    private readonly DynamicValuesPickerField _email = new DynamicValuesPickerField("Emails")
    {
      SelectedResolver = Pickers.Misc.Email
    };

    private readonly TextField _nonEmptyText =
      new TextField(displayName: "Guess what", validator: FieldValidators.IsMatches(@"^\d{1,}$"));

    private readonly IPasswordGenerator _passwordGenerator;
    private readonly IMailsWatcher _mailsWatcher;
    private readonly ISvdAccountConfirmationService _accountConfirmationService;

    public SvdAccountsGenerator(IPasswordGenerator passwordGenerator, IMailsWatcher mailsWatcher,
      ISvdAccountConfirmationService accountConfirmationService)
    {
      _passwordGenerator = passwordGenerator;
      _mailsWatcher = mailsWatcher;
      _accountConfirmationService = accountConfirmationService;
    }

    public override IEnumerable<Field> ConfigurationFields
    {
      get
      {
        yield return _email;
        yield return _nonEmptyText;
      }
    }

    public override async IAsyncEnumerable<Account> GenerateAsync(
      [EnumeratorCancellation] CancellationToken ct = default)
    {
      while (!ct.IsCancellationRequested)
      {
        await Task.Delay(TimeSpan.FromSeconds(1), ct);
        var email = _email.Value;
        Console.WriteLine(Profile.ProfileName);
        // if (email.CanBeTracked())
        // {
        //   await ConfirmEmailAsync(email, ct);
        // }

        yield return new Account(email/*.Value*/, _passwordGenerator.Generate(PasswordLen));
      }
    }

    private async Task ConfirmEmailAsync(Email email, CancellationToken ct)
    {
      await foreach (var message in _mailsWatcher.WatchAsync(email, ct: ct))
      {
        if (_accountConfirmationService.IsConfirmationMessage(email, message))
        {
          await _accountConfirmationService.ConfirmAsync(email, message, ct);
          break;
        }
      }
    }
  }
}