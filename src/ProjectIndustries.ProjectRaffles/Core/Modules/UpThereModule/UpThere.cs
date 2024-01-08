using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.ShopifyAccountGenerator;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.UpThereModule
{
  [RaffleModuleName("UpThere")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.Custom)]
  [RaffleRegion(RaffleRegion.Oceania)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleAccountGenerator(typeof(ShopifyAccountGenerator))]
  public class UpThere : AccountBasedRaffleModuleBase<IUpThereClient>
  {
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };

    private readonly AddressFields _addressFields = new AddressFields
    {
      FirstName = null,
      LastName = null
    };

    private readonly CreditCardFields _creditCardFields = new CreditCardFields();

    public UpThere(IUpThereClient client)
      : base(client, @"https:\/\/uptherestore\.com\/.*", true)
    {
      AuthenticationConfig =
        new AuthenticationConfigHolder(Client.LoginAsync, AuthenticationConfigHolder.NoopCredentialsConfigurer);
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _addressFields;
      yield return _creditCardFields;
    }

    protected override async Task<Product> FetchProductAsync(CancellationToken ct)
    {
      var product = await Client.GetProductAsync(RaffleUrl, ct);
      return new Product {Name = product};
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.LoggingIntoAccount;
      await Client.LoginAsync(SelectedAccount, ct);
      
      Status = RaffleStatus.GettingRaffleInfo;
      var raffleEndpoint = await Client.GetRaffleEndpointAsync(RaffleUrl, ct);

      var product = await Client.GetProductAsync(_addressFields, raffleEndpoint, ct);

      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(_addressFields, _creditCardFields, SelectedAccount, product, raffleEndpoint,
        _sizeValue.Value, ct);
    }
  }
}