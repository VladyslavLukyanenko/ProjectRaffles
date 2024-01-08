using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.StarWarsCelebrationModule
{
  [RaffleModuleName("StarWarsCelebration")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class StarWarsCelebration : EmailBasedRaffleModuleBase<IStarWarsCelebrationClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      PhoneNumber = null,
      FullName = null,
      AddressLine1 = null,
      AddressLine2 = null,
      PostCode = null,
      City = null,
      ProvinceId = null
    };

    public StarWarsCelebration(IStarWarsCelebrationClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https:\/\/www\.starwarscelebration\.com\/.*")
    {
      _captchaSolver = captchaSolver;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _addressFields;
    }

    protected override async Task<Product> FetchProductAsync(CancellationToken ct)
    {
      var title = await Client.GetProductAsync(RaffleUrl, ct);
      return new Product{Name = title};
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parsed = await Client.ParseRaffleAsync(RaffleUrl, ct);

      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha =
        await _captchaSolver.SolveReCaptchaV2Async("6LcrdB4UAAAAAMmYQro49ql5DsIuOCqVL8wKqWz2", RaffleUrl, true, ct);
      
      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(_addressFields, parsed, EmailField, captcha, ct);
    }
  }
}