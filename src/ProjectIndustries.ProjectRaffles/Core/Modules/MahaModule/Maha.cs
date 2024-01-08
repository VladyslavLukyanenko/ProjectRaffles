using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;
using ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.MahaAccountGenerator;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.MahaModule
{
  [RaffleModuleName("Maha")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  [RaffleAccountGenerator(typeof(MahaAccountGenerator))]
  public class Maha : AccountBasedRaffleModuleBase<IMahaClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly DynamicValuesPickerField _instagram = new DynamicValuesPickerField("instagramHandle", "Instagram", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly TextField _shipping = new TextField("shipping", "Shipping Type", true);
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      ProvinceId = null,
      City = null,
      PostCode = null,
      AddressLine1 = null,
      AddressLine2 = null
    };

    public Maha(IMahaClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https://shop.maha-amsterdam.com/.*")
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
      var product = await Client.FetchProductAsync(RaffleUrl, ct);
      
      return new Product { Name = product};
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parseRaffle = await Client.ParseRaffleAsync(RaffleUrl, ct);

      var captcha =
        await _captchaSolver.SolveReCaptchaV2Async("6LfEefkUAAAAAKlfwnsHEEcvqAfef0Q7PG5cYJcC", RaffleUrl, true, ct);
      
      Status = RaffleStatus.Submitting;
      var payload = new MahaSubmitPayload(_addressFields, SelectedAccount, parseRaffle, _sizeValue.Value, _instagram.Value, captcha, _shipping.Value);
      return await Client.SubmitAsync(payload, ct);
    }
  }
}