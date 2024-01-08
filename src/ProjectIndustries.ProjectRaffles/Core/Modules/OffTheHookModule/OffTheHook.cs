using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.OffTheHookModule
{
  [RaffleModuleName("Off The Hook")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.MailChimp)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.NorthAmerica)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class OffTheHook : EmailBasedRaffleModuleBase<IOffTheHookClient>
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
      City = null,
      FullName = null,
      ProvinceId = null,
      AddressLine1 = null,
      AddressLine2 = null,
      CountryId = null,
      PostCode = null
    };


    public OffTheHook(IOffTheHookClient client)
      : base(client, @"https:\/\/offthehook\.ca\/pages\/.*")
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
      var parsedProduct = await Client.ParseSiteAsync(RaffleUrl, ct);

      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(_addressFields, EmailField, parsedProduct, _sizeValue.Value,
        _instagramHandle.Value,
        ct);
    }
  }
}