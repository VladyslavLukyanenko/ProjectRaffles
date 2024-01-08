using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TikeModule
{
  [DisabledRaffleModule]
  [RaffleModuleName("Tike")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class Tike : EmailBasedRaffleModuleBase<ITikeClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;

    private readonly ICountriesService _countriesService;
    
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly DynamicValuesPickerField _houseNumber = new DynamicValuesPickerField("houseNumber", "Street Number", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      AddressLine2 = null
    };
    
    public Tike(ITikeClient client, ICaptchaSolveService solveService, ICountriesService countriesService)
      : base(client, @"https:\/\/www\.tike\.ro\/.*")
    {
      _countriesService = countriesService;
      _captchaSolver = solveService;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _houseNumber;
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
      var state = _countriesService.GetProvinceName(_addressFields.CountryId.Value, _addressFields.ProvinceId.Value);
      var regionId = await Client.GetRegionId(RaffleUrl, state, ct);
      
      Status = RaffleStatus.SolvingCAPTCHA;
      var captchaImage = await Client.GetCaptchaImage(RaffleUrl, ct);
      var captcha = await _captchaSolver.SolveImageCaptchaAsync(captchaImage, ct);
      
      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(_addressFields, EmailField, RaffleUrl, captcha, regionId, _houseNumber.Value, _sizeValue.Value, ct);
    }
  }
}