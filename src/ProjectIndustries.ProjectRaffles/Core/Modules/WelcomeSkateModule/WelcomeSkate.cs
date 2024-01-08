using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WelcomeSkateModule
{
  [RaffleModuleName("WelcomeSk8")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class WelcomeSkate : EmailBasedRaffleModuleBase<IWelcomeSkateClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };

    private readonly AddressFields _addressFields = new AddressFields
    {
      FirstName = null,
      LastName = null,
      AddressLine1 = null,
      AddressLine2 = null,
      ProvinceId = null,
      City = null,
      PostCode = null
    };
    

    public WelcomeSkate(IWelcomeSkateClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https:\/\/welcomesk8\.com\/blogs\/.*")
    {
      _captchaSolver = captchaSolver;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
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
      var parsedRaffle = await Client.GetIdxValuesAsync(RaffleUrl, ct);

      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha = await _captchaSolver.SolveReCaptchaV2Async("6LfD8-IUAAAAANBE5-Xu-YCxacsAp0t74MR3CooL",
        $"https://www.powr.io/form-builder/u/{parsedRaffle.RaffleId}", false, ct);
      
      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(_addressFields, parsedRaffle, EmailField, captcha, _sizeValue.Value, ct);
    }
  }
}