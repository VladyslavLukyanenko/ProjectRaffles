using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.FenomAccountGenerator
{
  public class FenomAccountGenerator : ProfileDependentAccountGeneratorBase
  {
    private readonly IFenomAccountGeneratorClient _client;
    
    private readonly RadioButtonGroupField<string> _gender = new RadioButtonGroupField<string>("prefix",
      new Dictionary<string, string>
      {
        {"Ms.", "2"},
        {"Mr.", "1"},
        {"Miss", "3"},
      }, "Prefix");

    private readonly DynamicValuesPickerField _email =
      new DynamicValuesPickerField("email", "Emails", true, null, Pickers.All)
      {
        SelectedResolver = Pickers.Misc.Email
      };
    
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

    public FenomAccountGenerator(IFenomAccountGeneratorClient client)
    {
      _client = client;
    }

    public override IEnumerable<Field> ConfigurationFields
    {
      get
      {
        yield return _gender;
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
        var email = _email.Value;
        
        _client.GenerateHttpClient();

        var submitAccount = await _client.SubmitAccount(_addressFields, email, _gender.Value);
        
        if(!submitAccount) throw new RaffleFailedException("error on account creation","Error on account creation");
        yield return new Account(email, "ProjectRaffles!1)3!");
      }
    }
  }
}