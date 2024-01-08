using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SnipesUsaModule
{
  [RaffleModuleName("Snipes USA")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.NorthAmerica)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class SnipesUsa : EmailBasedRaffleModuleBase<ISnipesUsaClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly TextField _model = new TextField(displayName: "Model", isRequired: true);
    
    private readonly TextField _store = new TextField(displayName: "Store (Without 'Snipes - ')!", isRequired: true);

    private readonly AddressFields _addressFields = new AddressFields
    {
      FullName = null,
      CountryId = null,
      City = null,
      AddressLine1 = null,
      AddressLine2 = null,
      ProvinceId = null
    };
    
    public SnipesUsa(ISnipesUsaClient client, ICaptchaSolveService captchaSolver)
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
      yield return _addressFields;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      var product = _model.Value;
      return Task.FromResult(new Product {Name = product});
    }
    
    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parsedRaffle = await Client.ParseSnipesApiAsync(ct);

      var sizeGuid = await Client.GetSizeIdAsync(parsedRaffle, _model.Value, _sizeValue.Value, _store.Value);
      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha = await _captchaSolver.SolveReCaptchaV2Async("6Lf1zbMUAAAAANBwSjY8Mh5d0bTe4-ucx5Gt1UEz", "https://raffle.snipesusa.com/signup", false, ct);
      Status = RaffleStatus.Submitting; 
      return await Client.SubmitAsync(_addressFields, EmailField, sizeGuid, captcha, ct);
    }
  }
}