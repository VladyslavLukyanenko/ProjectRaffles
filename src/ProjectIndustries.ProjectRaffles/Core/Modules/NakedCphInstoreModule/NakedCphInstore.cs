using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NakedCphInstoreModule
{
  [RaffleModuleName("NakedCPH INSTORE")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class NakedCphInstore : EmailBasedRaffleModuleBase<INakedCphInstoreClient>
  {

    private readonly TextField _raffleTag = new TextField(displayName: "Raffle tag");
    
    private readonly DynamicValuesPickerField _size = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };

    private readonly AddressFields _addressFields = new AddressFields();
    
    private readonly DynamicValuesPickerField _instagramHandle = new DynamicValuesPickerField("instagramHandle", "Instagram", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };

    public NakedCphInstore(INakedCphInstoreClient client)
      : base(client, @"https:\/\/www\.nakedcph\.com\/.*")
    {
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _raffleTag;
      yield return _size;
      yield return _instagramHandle;
      yield return _addressFields;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product {Name = "NakedCPH Instore Raffle"});
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(_addressFields, EmailField, _instagramHandle.Value, _raffleTag.Value, _size.Value, ct);
    }
  }
}