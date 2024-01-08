using System.Collections.Generic;
using System.IO.Packaging;
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

namespace ProjectIndustries.ProjectRaffles.Core.Modules.Raffle43einhalbModule
{
  [RaffleModuleName("43einhalb")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class Raffle43einhalb : EmailBasedRaffleModuleBase<IRaffle43einhalbClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", " EU Size (only numbers)", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    private readonly TextField _houseNumber = new TextField(displayName: "House Number");
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      FullName = null,
      ProvinceId = null
    };
    
    public Raffle43einhalb(IRaffle43einhalbClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https:\/\/releases\.43einhalb\.com\/.*")
    {
      _captchaSolver = captchaSolver;
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
      var product = await Client.GetProductNameAsync(RaffleUrl, ct);
      return new Product {Name = product};
    }
    
    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parseRaffle = await Client.ParseRaffleAsync(RaffleUrl, _sizeValue.Value, ct);

      Status = RaffleStatus.SolvingCAPTCHA;
      var captchaSite = "https://releases.43einhalb.com/raffle-form?productBsId=" + parseRaffle;
      var captcha = await _captchaSolver.SolveReCaptchaV3Async("6Ld7ha8UAAAAAF1XPgAtu53aId9_SMkVMmsK1hyK", captchaSite, "submit", 0.3, ct);
      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(_addressFields, _houseNumber.Value, EmailField, parseRaffle, captcha, ct);
    }
  }
}