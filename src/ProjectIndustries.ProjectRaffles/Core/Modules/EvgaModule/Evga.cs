using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.EvgaModule
{
  [RaffleModuleName("EVGA")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  //[RaffleAccountGenerator(typeof(EvgaAccountGenerator))]
  [RaffleRegion(RaffleRegion.NorthAmerica)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class Evga : AccountBasedRaffleModuleBase<IEvgaClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    public Evga(IEvgaClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https:\/\/www\.evga\.com\/products\/auto.otify\.aspx\?pn=.*")
    {
      _captchaSolver = captchaSolver;
    }
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      AddressLine1 = null,
      AddressLine2 = null,
      ProvinceId = null,
      PostCode = null,
      City = null,
      CountryId = null
    };
    
    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
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
      var parsedFields = await Client.ParseProductAsync(RaffleUrl, ct);

      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha =
        await _captchaSolver.SolveReCaptchaV2Async("6Lf22-sSAAAAACayYWhxiHN_554ipXEU4bjwQjY7", RaffleUrl, true,
          ct);

      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(_addressFields, SelectedAccount, RaffleUrl, parsedFields, captcha, ct);
    }
  }
}