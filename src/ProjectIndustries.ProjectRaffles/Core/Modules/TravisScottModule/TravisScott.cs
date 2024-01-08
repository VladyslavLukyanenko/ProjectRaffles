using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TravisScottModule
{
  [RaffleModuleName("Travis Scott")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.NorthAmerica)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class TravisScott : EmailBasedRaffleModuleBase<ITravisScottClient>
  {
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      AddressLine1 = null,
      AddressLine2 = null,
      ProvinceId = null,
      CountryId = null,
      City = null
    };

    public TravisScott(ITravisScottClient client)
      : base(client, @"https:\/\/shop\.travisscott\.com\/.*")
    {
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _addressFields;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product {Name = "Travis Scott Raffle"});
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parsedRaffle = await Client.ParseRaffleAsync(RaffleUrl, ct);
      
      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(RaffleUrl, EmailField, _addressFields, parsedRaffle, _sizeValue.Value, ct);
    }
  }
}