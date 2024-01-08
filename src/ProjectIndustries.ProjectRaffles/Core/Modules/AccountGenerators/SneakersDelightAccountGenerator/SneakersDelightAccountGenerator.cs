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

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.SneakersDelightAccountGenerator
{
  public class SneakersDelightAccountGenerator : ProfileDependentAccountGeneratorBase
  {
    private readonly ISneakersDelightAccountGeneratorClient _client;
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
      City = null
    };

    private readonly RadioButtonGroupField<string> _gender = new RadioButtonGroupField<string>("gender",
      new Dictionary<string, string>
      {
        {"Female", "2"},
        {"Male", "1"},
        {"NB", "3"},
      }, "Gender");
    
    public SneakersDelightAccountGenerator(ISneakersDelightAccountGeneratorClient client, ICaptchaSolveService captchaSolver)
    {
      _client = client;
      _captchaSolver = captchaSolver;
    }

    public override IEnumerable<Field> ConfigurationFields
    {
      get
      {
        yield return _email;
        yield return _gender;
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
        
        var parsed = await _client.ParseLoginAsync(ct);

        var captcha = await _captchaSolver.SolveReCaptchaV3Async("6LeVmPkUAAAAAIhCpWvdie7d7XJzW2bnpjWOO4nC",
          "https://sneakersdelight.store/customer/account/login/", "verify",0.3, ct);
        
        var submit2 = await _client.SubmitAccountAsync(parsed, _addressFields, email, _gender.Value, captcha, ct);

        if(!submit2) throw new RaffleFailedException("error on account creation","Error on account creation");
        yield return new Account(email, "ProjectRaffles!1)3!");
      }
    }
  }
}