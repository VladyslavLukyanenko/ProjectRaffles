using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AtmosModule
{
  [RaffleModuleName("Atmos US + Ubiq")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.NorthAmerica)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class Atmos : EmailBasedRaffleModuleBase<IAtmosClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size (random for totally random)", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly TextField _model = new TextField(displayName: "Model", isRequired: true);
    
    private readonly TextField _store = new TextField(displayName: "Store", isRequired: true);
    
    private readonly DynamicValuesPickerField _instagramHandle = new DynamicValuesPickerField("instagramHandle", "Instagram", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };

    private readonly AddressFields _addressFields = new AddressFields
    {
      FullName = null,
      CountryId = null
    };
    
    private readonly RadioButtonGroupField<string> _useAddress = new RadioButtonGroupField<string>("useAddress",
      new Dictionary<string, string>
      {
        {"Yes", "Y"},
        {"No", "N"},
      }, "Submit Address?", false);
    
    public Atmos(IAtmosClient client, ICaptchaSolveService captchaSolver)
      : base(client, @".*")
    {
      _captchaSolver = captchaSolver;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _model;
      yield return _store;
      yield return _instagramHandle;
      yield return _addressFields;
      yield return _useAddress;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      var product = _model.Value;
      return Task.FromResult(new Product {Name = product});
    }
    
    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parsedRaffle = await Client.ParseApiAsync(ct);

      var model = _model.Value.ToLower();
      var sizeAndModel = await Client.GetSizeAndModelAsync(parsedRaffle, model, _sizeValue.Value, _store.Value);
      
      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha = await _captchaSolver.SolveReCaptchaV2Async("6LcES6sZAAAAACGrj8rrJm13isYkVke0iBaZwtaF", "https://releases.atmosusa.com/entry", false, ct);
      Status = RaffleStatus.Submitting; 
      return await Client.SubmitAsync(_addressFields, sizeAndModel, captcha, EmailField, _instagramHandle.Value, _useAddress.Value, ct);
    }
  }
}