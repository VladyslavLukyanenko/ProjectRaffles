using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.ContinuumModule
{
  [RaffleModuleName("Continuum")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.NorthAmerica)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class Continuum : EmailBasedRaffleModuleBase<IContinuumClient>
  {
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };

    private readonly DynamicValuesPickerField _instagramHandle =
      new DynamicValuesPickerField("instagramHandle", "Instagram", true, null, Pickers.All)
      {
        // SelectedResolver = Pickers.Misc.ListItem
      };
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      CountryId = null
    };
    
    public Continuum(IContinuumClient client)
      : base(client, @"https:\/\/continuum-skateshop\.myshopify\.com\/pages\/.*")
    {
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _instagramHandle;
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
      var parsedRaffle = await Client.GetFormIdAsync(RaffleUrl, ct);

      var jsonContent = await Client.CraftJsonContent(_addressFields, _sizeValue.Value, EmailField);

      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(parsedRaffle, jsonContent, _addressFields, EmailField, ct);
    }
  }
}