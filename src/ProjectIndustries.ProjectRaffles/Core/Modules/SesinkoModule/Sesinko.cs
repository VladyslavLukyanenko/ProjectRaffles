using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SesinkoModule
{
  [RaffleModuleName("Sesinko")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class Sesinko : EmailBasedRaffleModuleBase<ISesinkoClient>
  {
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly DynamicValuesPickerField _instagramHandle = new DynamicValuesPickerField("instagramHandle", "Instagram", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly DynamicValuesPickerField _pickup = new DynamicValuesPickerField("pickup", "Pick Up Location", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      AddressLine1 = null,
      FirstName = null,
      LastName = null,
      CountryId = null,
      AddressLine2 = null,
      PhoneNumber = null,
      ProvinceId = null
    };

    public Sesinko(ISesinkoClient client)
      : base(client, @"https:\/\/sesinko\.com\/.*")
    {
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _instagramHandle;
      yield return _pickup;
      yield return _addressFields;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product
      {
        Name = "Sesinko raffle"
      });
    }
    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parsed = await Client.ParseRaffleAsync(RaffleUrl, ct);


      var payload =
        new SesinkoSubmitPayload(_addressFields, EmailField.Value, parsed, RaffleUrl, _sizeValue.Value, _instagramHandle.Value, _pickup.Value);
      var formdata = await Client.CraftFormDataAsync(payload);

      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(formdata, payload, ct);
    }
  }
}