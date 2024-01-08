using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.BrownsFashionAccountGenerator
{
  public class BrownsFashionAccountGenerator : ProfileDependentAccountGeneratorBase
  {
    private readonly IBrownsFashionAccountGeneratorClient _client;

    private readonly DynamicValuesPickerField _email =
      new DynamicValuesPickerField("email", "Emails", true, null, Pickers.All)
      {
        SelectedResolver = Pickers.Misc.Email
      };
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      FirstName = null,
      LastName = null,
      AddressLine1 = null,
      AddressLine2 = null,
      City = null,
      PhoneNumber = null,
      PostCode = null,
      CountryId = null,
      ProvinceId = null
    };

    public BrownsFashionAccountGenerator(IBrownsFashionAccountGeneratorClient client)
    {
      _client = client;
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
        var email = _email.Value;
        
        _client.GenerateHttpClient();

        var submitAccount = await _client.SubmitAccount(_addressFields, email, ct);
        
        if(!submitAccount) throw new RaffleFailedException($"Error on account creation for email {email}",$"Error on account creation for email {email}");
        yield return new Account(email, "ProjectRaffles!1)3!");
      }
    }
  }
}