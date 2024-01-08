using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AwolModule
{
  [RaffleModuleName("Awol")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class Awol : EmailBasedRaffleModuleBase<IAwolClient>
  {
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly TextField _pickupLocation = new TextField(displayName: "Pickup");

    private readonly AddressFields _addressFields = new AddressFields
    {
      AddressLine2 = null,
      ProvinceId = null,
      CountryId = null,
      PhoneNumber = null,
      PostCode = null,
      City = null
    };

    public Awol(IAwolClient client)
      : base(client, @"http:\/\/www.awolraffles.com\/.*")
    {
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return _sizeValue;
      yield return _pickupLocation;
      yield return _addressFields;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product
      {
        Name = "AWOL Raffle"
      });
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parseRaffle = await Client.ParseRaffleAsync(RaffleUrl, ct);

      Status = RaffleStatus.Submitting;
      var payload =
        new AwolSubmitPayload(_addressFields, EmailField, RaffleUrl, _sizeValue.Value, _pickupLocation.Value, parseRaffle);
      return await Client.SubmitAsync(payload, ct);
    }
  }
}