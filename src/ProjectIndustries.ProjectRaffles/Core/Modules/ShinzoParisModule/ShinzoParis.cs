using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.ShinzoParisModule
{
  [DisabledRaffleModule]
  [RaffleModuleName("Shinzo Paris")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class ShinzoParis : EmailBasedRaffleModuleBase<IShinzoParisClient>
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
    
    private readonly RadioButtonGroupField<string> _shippingType = new RadioButtonGroupField<string>("shippingType", new Dictionary<string, string>
    {
      {"Online", "Online"},
      {"Instore", "Instore"},
    }, "Shipping type");
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      FullName = null,
      AddressLine2 = null,
      ProvinceId = null
    };

    public ShinzoParis(IShinzoParisClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https:\/\/raffle\.shinzo\.paris\/.*")
    {
      _captchaSolver = captchaSolver;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _instagramHandle;
      yield return _sizeValue;
      yield return _shippingType;
      yield return _addressFields;
    }

    protected override async Task<Product> FetchProductAsync(CancellationToken ct)
    {
      var product = await Client.GetProductAsync(RaffleUrl, ct);
      return new Product {Name = product};
    }
    
    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    { 
      Status = RaffleStatus.GettingRaffleInfo;
      var parsed = await Client.ParseRaffleAsync(RaffleUrl, ct);

      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha = await _captchaSolver.SolveReCaptchaV2Async("6LczOjoUAAAAABEfbqdtD11pFD5cZ0n5nhz89nxI",RaffleUrl,true, ct);

      Status = RaffleStatus.Submitting;
      var payload = new ShinzoParisSubmitPayload(_addressFields, parsed, _shippingType.Value, _sizeValue.Value, _instagramHandle.Value, captcha, EmailField);
      return await Client.SubmitAsync(payload, ct);
    }
  }
}