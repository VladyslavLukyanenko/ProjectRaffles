using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.VoostoreBerlinModule
{
  [RaffleModuleName("Voostore Berlin")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class VoostoreBerlin : EmailBasedRaffleModuleBase<IVoostoreBerlinClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;

    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly TextField _houseNumber = new TextField(displayName: "House Number");
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      FullName = null,
      AddressLine2 = null,
      ProvinceId = null
    };

    public VoostoreBerlin(IVoostoreBerlinClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https://raffle.vooberlin.com/index.php?.*")
    {
      _captchaSolver = captchaSolver;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _houseNumber;
      yield return _sizeValue;
      yield return _addressFields;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product
      {
        Name = "Voostore raffle"
      });
    }
    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var raffleDetails = await Client.GetRaffleTagsAsync(RaffleUrl, ct);
      
      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha =
        await _captchaSolver.SolveReCaptchaV3Async("6LcCnPwaAAAAACywiV7MQapF0CCOu1YtLYCEZRj-", RaffleUrl, "contact",0.3,
          ct);

      Status = RaffleStatus.Submitting;
      var payload = new VoostoreBerlinSubmitPayload(_addressFields, EmailField, captcha, _sizeValue.Value, RaffleUrl, raffleDetails,
        _houseNumber.Value);
      return await Client.SubmitAsync(payload, ct);
    }
  }
}