using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WoodWoodInstoreModule
{
  [RaffleModuleName("WoodWood Instore")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.RaffleFcfs)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class WoodWoodInstore : EmailBasedRaffleModuleBase<IWoodWoodInstoreClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    
    private readonly TextField _raffleTag = new TextField(displayName: "Raffle tag");
    
    private readonly DynamicValuesPickerField _size = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };

    private readonly AddressFields _addressFields = new AddressFields
    {
      FullName = null,
      ProvinceId = null
    };
    
    private readonly RadioButtonGroupField<string> _store = new RadioButtonGroupField<string>("store", 
      new Dictionary<string, string> 
      {
     {"Copenhagen", "G1"},
     {"Aarhus", "Aarhus"},
     {"Berlin", "Berlin"},
  }, "Store");
    

    public WoodWoodInstore(IWoodWoodInstoreClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https:\/\/www\.woodwood\.com\/.*")
    {
      _captchaSolver = captchaSolver;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _raffleTag;
      yield return _size;
      yield return _store;
      yield return _addressFields;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      //var product = await Client.GetProductAsync(GetSanitizedUrlWithoutQuery(), ct);
      return Task.FromResult(new Product {Name = "WoodWood Online Raffle"});
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var phoneCode = await Client.GetPhoneCodeAsync(_addressFields.CountryId.Value, ct);
      
      var ip = await Client.GetIPAsync(ct);

      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha =
        await _captchaSolver.SolveReCaptchaV2Async("6LfbPnAUAAAAACqfb_YCtJi7RY0WkK-1T4b9cUO8", RaffleUrl, true,
          ct);

      Status = RaffleStatus.Submitting;
      var payload = new WoodWoodInstorePayload(_addressFields, EmailField, _raffleTag.Value, captcha, ip, phoneCode, _store.Value, _size.Value);
      return await Client.SubmitAsync(payload, ct);
    }
  }
}