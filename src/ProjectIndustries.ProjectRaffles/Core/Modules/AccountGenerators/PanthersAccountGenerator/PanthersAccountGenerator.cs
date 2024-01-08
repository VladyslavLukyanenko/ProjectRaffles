using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.PanthersAccountGenerator
{
  public class PanthersAccountGenerator : ProfileDependentAccountGeneratorBase
  {
    private readonly ICaptchaSolveService _captchaSolver;
    private readonly IPanthersAccountGeneratorClient _client;

    private readonly DynamicValuesPickerField _email =
      new DynamicValuesPickerField("email", "Emails", true, null, Pickers.All)
      {
        SelectedResolver = Pickers.Misc.Email
      };

    private readonly TextField _houseNumber = new TextField(displayName: "Housenumber", isRequired: true);

    private readonly AddressFields _addressFields = new AddressFields
    {
      FullName = null,
      ProvinceId = null
    };

    public PanthersAccountGenerator(IPanthersAccountGeneratorClient client, ICaptchaSolveService captchaSolver)
    {
      _client = client;
      _captchaSolver = captchaSolver;
    }

    public override IEnumerable<Field> ConfigurationFields
    {
      get
      {
        yield return _email;
        yield return _houseNumber;
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

        var countries = await _client.GetCountriesAsync(ct);

        var country = _addressFields.CountryId.Value.ToLower();
        countries.CountryDictionary.TryGetValue(country, out string countryCode);
        countries.CountryType.TryGetValue(country, out string countryType);

        var phoneCode = await _client.GetPhoneCodeAsync(country, ct);

        var key = await _client.SubmitAccountAsync(_addressFields, email, countryType, _houseNumber, countryCode, phoneCode, ct);

        var captcha = await _captchaSolver.SolveReCaptchaV2Async("6LcSoR4UAAAAAO3mKqp729zO-PAz1m7DK9AqnONr",
          "https://www.panthers.be/en/services/challenge/", true, ct);

        var submit2 = await _client.SubmitCaptchaAsync(key, captcha, ct);
        if(!submit2) throw new RaffleFailedException("error on account creation","Error on account creation");
        yield return new Account(email, "ProjectRaffles!1)3!");
      }
    }
  }
}