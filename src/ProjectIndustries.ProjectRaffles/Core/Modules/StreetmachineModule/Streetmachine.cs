using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.StreetmachineAccountGenerator;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.StreetmachineModule
{
  [RaffleModuleName("Streetmachine")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.Custom)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleAccountGenerator(typeof(StreetmachineAccountGenerator))]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class Streetmachine : AccountBasedRaffleModuleBase<IStreetmachineClient>
  {
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };

    private readonly ICaptchaSolveService _captchaSolver;
    
    public Streetmachine(IStreetmachineClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https:\/\/www\.streetmachine\.com\/draw\/.*", true)
    {
      AuthenticationConfig =
        new AuthenticationConfigHolder(Client.LoginAsync, AuthenticationConfigHolder.NoopCredentialsConfigurer);

      _captchaSolver = captchaSolver;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      //var product = await Client.GetProductAsync(RaffleUrl, ct);
      return Task.FromResult( new Product {Name = "Streetmachine raffle"} );
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.LoggingIntoAccount;
      await Client.LoginAsync(SelectedAccount, ct);
      
      Status = RaffleStatus.GettingRaffleInfo;
      var product = await Client.ParseSizesAsync(RaffleUrl, _sizeValue.Value, ct);

      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha = await _captchaSolver.SolveReCaptchaV2Async("6LdrwtsUAAAAAGDa_VbZYCmmiFowrFZb_562hphY", RaffleUrl, true, ct);
      
      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(product, captcha, RaffleUrl, ct);
    }
  }
}