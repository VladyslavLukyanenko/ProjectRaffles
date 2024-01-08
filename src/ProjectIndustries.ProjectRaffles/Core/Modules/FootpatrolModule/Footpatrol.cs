using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.FootpatrolModule
{
  [DisabledRaffleModule]
  [RaffleModuleName("Footpatrol")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class Footpatrol : RaffleModuleBase
  {
    private readonly IFootpatrolClient _client;
    private readonly ICaptchaSolveService _captchaSolver;

    private readonly TextField _raffleUrl = new TextField("raffleUrl", "Raffle URL", true, FieldValidators.IsMatches(@"https:\/\/www\.footpatrol\.com\/page\/.*"));
    private readonly TextField _sizeValue = new TextField("sizeValue", "Size Value", true);
    private readonly TextField _county = new TextField("county", "County", true);
    private readonly DynamicValuesPickerField _email =
      new DynamicValuesPickerField("email", "Email", true, null, Pickers.All);
    private readonly DynamicValuesPickerField _paypalEmail =
      new DynamicValuesPickerField("paypalEmail", "Paypal Email", true, null, Pickers.All);

    public Footpatrol(IFootpatrolClient client, ICaptchaSolveService captchaSolver)
    {
      _client = client;
      _captchaSolver = captchaSolver;
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
      var size = await _client.SizeParserAsync(_raffleUrl.Value, _sizeValue.Value, ct);

      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha =
        await _captchaSolver.SolveReCaptchaV2Async("6LfIUL8UAAAAAMvYieKAMgh4e9qQFpLiLdqLLJG4", _raffleUrl.Value, true,
          ct);

      Status = RaffleStatus.Submitting;
      var payload = new FootpatrolSubmitPayload(profile, captcha, size, _raffleUrl.Value, _county.Value, _email.Value, _paypalEmail.Value);
      Status = await _client.SubmitAsync(payload, ct)
        ? RaffleStatus.Succeeded
        : RaffleStatus.Failed;
    }
  }
}