using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PhenomModule
{
  [RaffleModuleName("Phenom")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class Phenom : EmailBasedRaffleModuleBase<IPhenomClient>
  { 
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly DynamicValuesPickerField _instagramHandle = new DynamicValuesPickerField("instagramHandle", "Instagram", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      FirstName = null,
      LastName = null,
      AddressLine2 = null
    };
    
    public Phenom(IPhenomClient client)
      : base(client, @"https:\/\/phenomglobal\.com\/.*")
    {
    }
    
    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _instagramHandle;
      yield return _addressFields;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product
      {
        Name = "Phenom raffle"
      });
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parsed = await Client.ParseRaffleAsync(RaffleUrl, ct);

      Status = RaffleStatus.Submitting;
      var payload =
        new PhenomSubmitPayload(_addressFields, EmailField, parsed, RaffleUrl, _sizeValue.Value, _instagramHandle.Value);
      return await Client.SubmitAsync(payload, ct);
    }
  }
}