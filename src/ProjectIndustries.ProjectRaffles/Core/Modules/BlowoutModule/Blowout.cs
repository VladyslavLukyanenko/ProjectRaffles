using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.BlowoutModule
{
  [RaffleModuleName("Blowout")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class Blowout : EmailBasedRaffleModuleBase<IBlowoutClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;

    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly DynamicValuesPickerField _instagramHandle = new DynamicValuesPickerField("instagramHandle", "Instagram", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly TextField _pickup = new TextField("pickupLocation", "Pickup (Yes/No)", true);
    
    private readonly DynamicValuesPickerField _houseNumber = new DynamicValuesPickerField("houseNumber", "Housenumber", true,  null, Pickers.All){};
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      ProvinceId = null
    };

    public Blowout(IBlowoutClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https:\/\/www\.blowoutshop\.de\/.*")
    {
      _captchaSolver = captchaSolver;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _instagramHandle;
      yield return _pickup;
      yield return _houseNumber;
      yield return _addressFields;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product
      {
        Name = "Blowout Raffle"
      });
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var csrfToken = await Client.GenerateCsrfTokenAsync();

      var parsedRaffle = await Client.ParseRaffleAsync(RaffleUrl, ct);

     // var captchaString = await Client.GetCaptchaImage(ct);

     // Status = RaffleStatus.SolvingCAPTCHA;
     // var captcha = await _captchaSolver.SolveImageCaptchaAsync(captchaString, ct);


      Status = RaffleStatus.Submitting;
      var payload =
        new BlowoutSubmitPayload(_addressFields, EmailField, null, RaffleUrl, _sizeValue.Value, _instagramHandle.Value,
          _pickup.Value, parsedRaffle, csrfToken, _houseNumber.Value);

      return await Client.SubmitAsync(payload, ct);
    }
  }
}