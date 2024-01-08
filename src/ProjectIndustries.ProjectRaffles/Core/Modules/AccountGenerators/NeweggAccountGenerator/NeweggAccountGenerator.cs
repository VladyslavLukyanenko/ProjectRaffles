using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.NeweggAccountGenerator
{
  public class NeweggAccountGenerator : ProfileDependentAccountGeneratorBase
  {
    private readonly INeweggAccountGeneratorClient _client;
    private readonly ICaptchaSolveService _captchaSolver;

    private readonly DynamicValuesPickerField _email =
      new DynamicValuesPickerField("email", "Emails", true, null, Pickers.All)
      {
        SelectedResolver = Pickers.Misc.Email
      };
    
    private readonly RadioButtonGroupField<bool> _retryOnFailedCaptcha = new RadioButtonGroupField<bool>("Retry on failed CAPTCHA", new Dictionary<string, bool>
    {
      {"Yes", true},
      {"No", false}
    }, "Retry on failed CAPTCHA");
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      FullName = null,
      AddressLine1 = null,
      AddressLine2 = null,
      City = null,
      PhoneNumber = null,
      PostCode = null,
      CountryId = null,
      ProvinceId = null
    };

    public NeweggAccountGenerator(INeweggAccountGeneratorClient client, ICaptchaSolveService captchaSolver)
    {
      _client = client;
      _captchaSolver = captchaSolver;
    }

    public override IEnumerable<Field> ConfigurationFields
    {
      get
      {
        yield return _email;
        yield return _retryOnFailedCaptcha;
        foreach (Field field in _addressFields)
        {
          yield return field;
        }
      }
    }

    public override async IAsyncEnumerable<Account> GenerateAsync([EnumeratorCancellation] CancellationToken ct = default)
    {
      while (!ct.IsCancellationRequested)
      {
        var email = _email.Value;
        
        _client.GenerateHttpClient();

        var ticketId = await _client.GetTicketId(ct);

        var formData = await _client.ParseData(ticketId, ct);

        var captchaToken = await _captchaSolver.SolveReCaptchaV3Async("6LfgV6wUAAAAAJitb3pPw0WP9q8zACyxczUZk-P7",
          "https://secure.newegg.com/identity/signup?tk=" + ticketId, "Register", 0.9, ct);

        var submitted = await _client.SubmitAccount(_addressFields, email, captchaToken, formData, ticketId, ct);

        string submissionResult = submitted.Result;
        
        
        if (submissionResult == "ReCaptchaFailed" && _retryOnFailedCaptcha.Value)
        {
          while (submissionResult == "ReCaptchaFailed")
          {
            captchaToken = await _captchaSolver.SolveReCaptchaV3Async("6LfgV6wUAAAAAJitb3pPw0WP9q8zACyxczUZk-P7", "https://secure.newegg.com/identity/signup?tk=" + ticketId, "Register", 0.9, ct);
            submitted = await _client.SubmitAccount(_addressFields, email, captchaToken, formData, ticketId, ct);
            submissionResult = submitted.Result;
          }
        }
        
        if (submissionResult == "SignUpFailure")
        {
          throw new RaffleFailedException("Account already exists", "Account with email: " + email + " already exists");
        }

        if (submissionResult == "ServiceError")
        {
          throw new RaffleFailedException("Newegg encountered an error", "Newegg encountered an error, newegg code: ServiceError");
        }
    
        if (submissionResult == "TicketExpired")
        {
          throw new RaffleFailedException("Ticket expired, try again", "Ticket expired, try again, code: TicketExpired");
        }
        
        

        if (submissionResult == "Success")
        {
          string cookieString = await _client.GetAccountCookies(submitted, ct);
          yield return new Account(email, "ProjectRaffles123!", cookieString);
        } 
      }
    }
  }
}