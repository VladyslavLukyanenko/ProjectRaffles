using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.PanthersAccountGenerator;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PanthersModule
{
  [RaffleModuleName("Panthers")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleAccountGenerator(typeof(PanthersAccountGenerator))]
  public class Panthers : AccountBasedRaffleModuleBase<IPanthersClient>
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
      FullName = null,
      AddressLine2 = null,
      ProvinceId = null
    };

    public Panthers(IPanthersClient client)
      : base(client, @"https:\/\/www\.panthers\.be\/.*")
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

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parsed = await Client.ParseRaffleAsync(RaffleUrl, ct);

      var payload = new PanthersSubmitPayload(_addressFields, SelectedAccount, parsed, RaffleUrl, _sizeValue.Value,
        _instagramHandle.Value);

      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(payload, ct);
    }
  }
}