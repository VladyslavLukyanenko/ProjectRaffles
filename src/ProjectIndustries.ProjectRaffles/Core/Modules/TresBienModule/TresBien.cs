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

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TresBienModule
{
  [RaffleModuleName("Tres Bien")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class TresBien : EmailBasedRaffleModuleBase<ITresBienClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      FullName = null,
      AddressLine2 = null,
      ProvinceId = null
    };

    public TresBien(ITresBienClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https:\/\/tres-bien\.com\/.*")
    {
      _captchaSolver = captchaSolver;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _addressFields;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product
      {
        Name = "Tres Bien Raffle"
      });
    }
    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var raffleDetails = await Client.GetRaffleTags(RaffleUrl, ct);

      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha =
        await _captchaSolver.SolveReCaptchaV2Async("6LfjzmQUAAAAAJxTOcx3vYq3hroeYczGfDPU-NlX", RaffleUrl, true, ct);

      Status = RaffleStatus.Submitting;
      var payload = new TresBienSubmitPayload(_addressFields, EmailField, captcha, _sizeValue.Value, RaffleUrl, raffleDetails);
      return await Client.SubmitAsync(payload, ct);
    }
  }
}