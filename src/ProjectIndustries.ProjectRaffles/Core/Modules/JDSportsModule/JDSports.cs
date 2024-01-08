using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.JDSportsModule
{
  [DisabledRaffleModule]
  [RaffleModuleName("JD Sports")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class JDSports : RaffleModuleBase
  {
    private readonly IJDSportsClient _client;

    private readonly TextField _raffleUrl = new TextField("raffleUrl", "Raffle URL", true, FieldValidators.IsMatches(@"https:\/\/raffles\.jdsports\..*\/.*"));
    private readonly TextField _sizeValue = new TextField("sizeValue", "Size Value", true);
    private readonly TextField _county = new TextField("county", "County", true);
    private readonly DynamicValuesPickerField _email =
      new DynamicValuesPickerField("email", "Email", true, null, Pickers.All);
    private readonly DynamicValuesPickerField _paypalEmail =
      new DynamicValuesPickerField("paypalEmail", "Paypal Email", true, null, Pickers.All);
    public JDSports(IJDSportsClient client)
    {
      _client = client;
    }

    public override IEnumerable<Field> AdditionalFields
    {
      get
      {
        yield return _raffleUrl;
        yield return _sizeValue;
        yield return _county;
        yield return _email;
        yield return _paypalEmail;
      }
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product
      {
        Name = "Fake Product",
        Picture = "/Assets/testProductPicture.png"
      });
    }
    

    protected override IModuleHttpClient HttpClient => _client;

    protected override async Task ExecuteAsync(Profile profile, CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var baseurl = await _client.GetBaseUrlAsync(_raffleUrl.Value);

      var storeCountry = await _client.GetStoreCountryAsync(baseurl);

      var sizeValue = await _client.SizeParserAsync(_raffleUrl.Value, _sizeValue.Value, ct);


      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha = await _client.SolveCaptchaAsync(_raffleUrl.Value, baseurl, ct);

      Status = RaffleStatus.Submitting;
      var payload = new JDSportsSubmitPayload(profile, captcha, sizeValue, _raffleUrl.Value, storeCountry, baseurl,
        _county.Value, _email.Value, _paypalEmail.Value);
      Status = await _client.SubmitAsync(payload, ct)
        ? RaffleStatus.Succeeded
        : RaffleStatus.Failed;
    }
  }
}