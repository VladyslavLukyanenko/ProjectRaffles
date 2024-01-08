using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.BrownsFashionAccountGenerator;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.BrownsFashionModule
{
  #if !DEBUG
  [DisabledRaffleModule]
  #endif
  [RaffleModuleName("Browns Fashion")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.Custom)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  [RaffleAccountGenerator(typeof(BrownsFashionAccountGenerator))]
  public class BrownsFashion : AccountBasedRaffleModuleBase<IBrownsFashionClient>
  {

    private readonly TextField _sizeValue = new TextField(displayName: "Sizevalue");

    private readonly AddressFields _addressFields = new AddressFields
    {
      AddressLine2 = null,
      ProvinceId = null,
      FullName = null
    };
    

    public BrownsFashion(IBrownsFashionClient client)
      : base(client, @"https:\/\/www\.brownsfashion\.com\/.*\/raffles\/.*", true)
    {
      AuthenticationConfig =
        new AuthenticationConfigHolder(Client.LoginAsync, AuthenticationConfigHolder.NoopCredentialsConfigurer);
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
      var raffleName = await Client.GetRaffleNameAsync(RaffleUrl, ct);

      var currencyCode = await Client.GetCurrencyCodeAsync(_addressFields.CountryId.Value, ct);
      
      Status = RaffleStatus.LoggingIntoAccount;
      await Client.LoginAsync(SelectedAccount, ct);

      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(_addressFields, SelectedAccount, raffleName, _sizeValue.Value, currencyCode, ct);
    }
  }
}