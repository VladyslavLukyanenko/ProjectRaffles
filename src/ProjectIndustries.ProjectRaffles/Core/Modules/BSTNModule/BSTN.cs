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

namespace ProjectIndustries.ProjectRaffles.Core.Modules.BSTNModule
{
  [DisabledRaffleModule]
  [RaffleModuleName("BSTN")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class BSTN : RaffleModuleBase
  {
    private readonly IBSTNClient _client;
    private readonly ICaptchaSolveService _captchaSolver;

    private readonly TextField _raffleUrl = new TextField("raffleUrl", "Raffle URL", true, FieldValidators.IsMatches(@"https:\/\/raffle\.bstn\.com\/.*"));
    private readonly TextField _instagramHandle = new TextField("instagramHandle", "Instagram Handle", true);
    private readonly TextField _sizeValue = new TextField("sizeValue", "Size, formatted like '45 1/3'", true);
    private readonly DynamicValuesPickerField _email =
      new DynamicValuesPickerField("email", "Email", true, null, Pickers.All);
    public BSTN(IBSTNClient client, ICaptchaSolveService captchaSolver)
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
        yield return _instagramHandle;
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
      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha =
        await _captchaSolver.SolveReCaptchaV2Async("6LemOsYUAAAAABGFj8B7eEvmFT1D1j8IbXJIdvNn", _raffleUrl.Value, true,
          ct);

      Status = RaffleStatus.Submitting;
      var payload = new BSTNSubmitPayload(profile, captcha, _sizeValue.Value, _instagramHandle.Value, _raffleUrl.Value, _email.Value);
      Status = await _client.SubmitAsync(payload, ct)
        ? RaffleStatus.Succeeded
        : RaffleStatus.Failed;
    }
  }
}