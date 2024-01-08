using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.InstagramAccountGenerator
{
  public class InstagramAccountGenerator : ProfileDependentAccountGeneratorBase
  {
    private readonly IInstagramAccountGeneratorClient _client;

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
    
    private readonly ProxyGroupPickerField _proxyPicker = new ProxyGroupPickerField("Proxy group");

    public InstagramAccountGenerator(IInstagramAccountGeneratorClient client)
    {
      _client = client;
    }

    public override IEnumerable<Field> ConfigurationFields
    {
      get
      {
        yield return _proxyPicker;
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
        //todo: create instagram account gen through SMS, since that email seemingly always needs checkpoint (I have only avoided checkpoint on some IP's)
        var email = _email.Value;
        
        _client.GenerateInstaApiHandler(email, _proxyPicker.GetNextAccount());

        await _client.CheckEmailAvailability(email);

        var submitAccount = await _client.CreateAccount(_addressFields, email, ct);
        
        if(!submitAccount) throw new RaffleFailedException("error on account creation","Error on account creation");
        yield return new Account(email, "ProjectRaffles!1)3!");
      }
    }
  }
}