using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SOTFModule
{
  [DisabledRaffleModule]
  [RaffleModuleName("SOTF")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class SOTF : RaffleModuleBase
  {
    private readonly ISOTFClient _client;
    private readonly ICaptchaSolveService _captchaSolver;

    private readonly TextField _colourway = new TextField("colourway", "Colourway", true);
    private readonly TextField _sizeValue = new TextField("sizeValue", "Size Value", true);
    
    private readonly DynamicValuesPickerField _instagramHandle = new DynamicValuesPickerField("instagramHandle", "Instagram", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    private readonly DynamicValuesPickerField _email =
      new DynamicValuesPickerField("email", "Email", true, null, Pickers.All);


    public SOTF(ISOTFClient client, ICaptchaSolveService captchaSolver)
    {
      _client = client;
      _captchaSolver = captchaSolver;
    }

    public override IEnumerable<Field> AdditionalFields
    {
      get
      {
        yield return _colourway;
        yield return _sizeValue;
        yield return _instagramHandle;
        yield return _email;
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
      var formid = await _client.ParseRaffleAsync(ct);

      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha = await _captchaSolver.SolveReCaptchaV2Async("6LeoeSkTAAAAAA9rkZs5oS82l69OEYjKRZAiKdaF",
        "https://www.sotf.com/en/raffle_online_form.php", true, ct);

      Status = RaffleStatus.Submitting;
      var payload = new SOTFSubmitPayload(profile, _colourway.Value, _sizeValue.Value, _instagramHandle.Value, captcha,
        formid, _email.Value);
      Status = await _client.SubmitAsync(payload, ct)
        ? RaffleStatus.Succeeded
        : RaffleStatus.Failed;
    }
  }
}