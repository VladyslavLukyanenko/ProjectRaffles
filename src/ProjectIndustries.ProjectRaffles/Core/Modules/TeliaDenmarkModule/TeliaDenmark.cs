using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TeliaDenmarkModule
{
  [RaffleModuleName("Telia Denmark")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class TeliaDenmark : EmailBasedRaffleModuleBase<ITeliaDenmarkClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      FirstName = null,
      LastName = null,
      AddressLine1 = null,
      AddressLine2 = null,
      PostCode = null,
      City = null,
      CountryId = null,
      ProvinceId = null
    };
    
    public TeliaDenmark(ITeliaDenmarkClient client, ICaptchaSolveService solveService)
      : base(client, @"https:\/\/www\.telia\.dk\/kampagner\/.*")
    {
      _captchaSolver = solveService;
    }

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
      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha =
        await _captchaSolver.SolveReCaptchaV2Async("6LeZ8-kZAAAAAB9kWzRfbON_0ByKz8aTijHlCXz5", RaffleUrl, true, ct);
      
      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(_addressFields, EmailField, RaffleUrl, captcha, ct);
    }
  }
}