using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.FenomAccountGenerator;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.FenomModule
{
  [RaffleModuleName("Fenom")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.Custom)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  [RaffleAccountGenerator(typeof(FenomAccountGenerator))]
  public class Fenom : AccountBasedRaffleModuleBase<IFenomClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "EU Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };

    private readonly AddressFields _addressFields = new AddressFields
    {
      FirstName = null,
      LastName = null,
      AddressLine1 = null,
      AddressLine2 = null,
      City = null,
      ProvinceId = null,
      CountryId = null,
      PostCode = null,
      PhoneNumber = null,
      FullName = null
    };
    

    public Fenom(IFenomClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https:\/\/www.fenom.com\/.*", true)
    {
      AuthenticationConfig =
        new AuthenticationConfigHolder(Client.LoginAsync, AuthenticationConfigHolder.NoopCredentialsConfigurer);
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
      var product = await Client.GetProductNameAsync(RaffleUrl, ct);
      return new Product {Name = product};
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.LoggingIntoAccount;
      await Client.LoginAsync(SelectedAccount, ct);
      
      Status = RaffleStatus.GettingRaffleInfo;
      var sizeIdentifiers = await Client.GetSizeIdentifiersAsync(RaffleUrl, _sizeValue.Value, ct);

      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha = await _captchaSolver.SolveReCaptchaV2Async("6Ld7VV0UAAAAAL81fvE4Lp0j30A5MiRzxbdiZVuG", RaffleUrl, true, ct);
      
      Status = RaffleStatus.Submitting;
      return await Client.SubmitEntryAsync(SelectedAccount, sizeIdentifiers, RaffleUrl, captcha, ct);
    }
  }
}