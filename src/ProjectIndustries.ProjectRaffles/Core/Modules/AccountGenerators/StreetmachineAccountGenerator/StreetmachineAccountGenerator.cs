using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.StreetmachineAccountGenerator
{
  public class StreetmachineAccountGenerator : ProfileDependentAccountGeneratorBase
  {
    private readonly IStreetmachineAccountGeneratorClient _client;
    private readonly ICaptchaSolveService _captchaSolver;

    private readonly DynamicValuesPickerField _email = new DynamicValuesPickerField("Emails")
    {
      SelectedResolver = Pickers.Misc.Email
    };

    private readonly AddressFields _addressFields = new AddressFields
    {
      FullName = null,
      AddressLine1 = null,
      AddressLine2 = null,
      ProvinceId = null,
      CountryId = null,
      PhoneNumber = null,
      City = null,
      PostCode = null
    };
    
    public StreetmachineAccountGenerator(IStreetmachineAccountGeneratorClient client, ICaptchaSolveService captchaSolver)
    {
      _client = client;
      _captchaSolver = captchaSolver;
    }

    public override IEnumerable<Field> ConfigurationFields
    {
      get
      {
        yield return _email;
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
        _client.GenerateHttpClient();
        
        var email = _email.Value;
        
        //get cookies
        await _client.GetCookies(ct);

        var captcha = await _captchaSolver.SolveReCaptchaV2Async("6LdrwtsUAAAAAGDa_VbZYCmmiFowrFZb_562hphY","https://www.streetmachine.com/register",true, ct);
        
        var submit2 = await _client.SubmitAccountAsync(_email.Value, _addressFields, captcha, ct);

        if(!submit2) throw new RaffleFailedException("error on account creation","Error on account creation");
        yield return new Account(email, "ProjectRaffles!1)3!");
      }
    }
  }
}