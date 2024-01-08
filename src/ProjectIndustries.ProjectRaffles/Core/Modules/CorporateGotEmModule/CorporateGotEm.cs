using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.CorporateGotEmModule
{
  [DisabledRaffleModule] //need to check if they have a selector field for instore.
  [RaffleModuleName("Corporate Got Em")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.NorthAmerica)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class CorporateGotEm : EmailBasedRaffleModuleBase<ICorporateGotEmClient>
  {
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };

    private readonly AddressFields _addressFields = new AddressFields
    {
      CountryId = null,
      AddressLine1 = null,
      AddressLine2 = null,
      PhoneNumber = null,
      ProvinceId = null,
      City = null
    };
    
    public CorporateGotEm(ICorporateGotEmClient client)
      : base(client, @"https:\/\/corporategotem\.com\/pages\/.*")
    {
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
    

    protected override async Task<bool> ExecuteAsync( CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parsedRaffle = await Client.ParseRaffleAsync(RaffleUrl, ct);
      
      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(_addressFields, parsedRaffle, EmailField, _sizeValue.Value, ct);
    }
  }
}