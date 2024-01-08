using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.Basket4BallersModule
{
  [DisabledRaffleModule] //might have gotten an anti-bot, need to check
  [RaffleModuleName("Basket 4 Ballers")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class Basket4Ballers : EmailBasedRaffleModuleBase<IBasket4BallersClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;

    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "EU Size (only numbers)", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };

    private readonly DynamicValuesPickerField _instagramHandle = new DynamicValuesPickerField("instagramHandle", "Instagram", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly AddressFields _addressFields = new AddressFields
    {
    };

    public Basket4Ballers(IBasket4BallersClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https:\/\/www\.basket4ballers\.com\/.*")
    {
      _captchaSolver = captchaSolver;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _instagramHandle;
      yield return _addressFields;
    }

    protected override async Task<Product> FetchProductAsync(CancellationToken ct)
    {
      var product =  await Client.GetProductAsync(RaffleUrl, ct);
      return new Product {Name = product};
    }
    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    { 
      Status = RaffleStatus.GettingRaffleInfo;
      var parsedProduct = await Client.ParseRaffleAsync(RaffleUrl, ct);

      var sizeId = await Client.FindSizeAsync(RaffleUrl, _sizeValue.Value, ct);

      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha = await _captchaSolver.SolveReCaptchaV2Async("6LcpBD0UAAAAALwqETJkSSuQZYcavdDKu1sy_XPN",RaffleUrl, true, ct);

      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(_addressFields, parsedProduct, sizeId, captcha, RaffleUrl, EmailField, _instagramHandle.Value, ct);
    }
  }
}