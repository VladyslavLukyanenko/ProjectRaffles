using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.ShopifyAccountGenerator
{
  public class ShopifyAccountGenerator : ProfileDependentAccountGeneratorBase
  {
    private readonly IShopifyAccountGeneratorClient _client;

    private readonly TextField _siteUrl =
      new TextField(displayName: "Site URL", isRequired: true);

    private readonly DynamicValuesPickerField _email = new DynamicValuesPickerField("Emails")
    {
      SelectedResolver = Pickers.Misc.Email
    };

    private readonly DynamicValuesPickerField _firstName = new DynamicValuesPickerField("First Name")
    {
      SelectedResolver = Pickers.ProfileFields.FirstName
    };

    private readonly DynamicValuesPickerField _lastName =
      new DynamicValuesPickerField("lastName", "Last Name", true, null, Pickers.All)
      {
        SelectedResolver = Pickers.ProfileFields.LastName
      };

    public ShopifyAccountGenerator(IShopifyAccountGeneratorClient client)
    {
      _client = client;
    }

    public override IEnumerable<Field> ConfigurationFields
    {
      get
      {
        yield return _siteUrl;
        yield return _email;
        yield return _firstName;
        yield return _lastName;
      }
    }

    public override async IAsyncEnumerable<Account> GenerateAsync([EnumeratorCancellation] CancellationToken ct = default)
    {
      while (!ct.IsCancellationRequested)
      {
        _client.GenerateHttpClient();
        
        var email = _email.Value;

        var uri = new Uri(_siteUrl.Value);
        var baseUrl = uri.Host;

        var key = await _client.PostAccountInfoAsync(baseUrl, _firstName.Value, _lastName.Value, email, ct);

        var submit2 = await _client.PostCaptchaAsync(baseUrl, key, ct);

        if(!submit2) throw new RaffleFailedException("error on account creation","Error on account creation");
        yield return new Account(email, "ProjectRaffles!1)3!");
      }
    }
  }
}