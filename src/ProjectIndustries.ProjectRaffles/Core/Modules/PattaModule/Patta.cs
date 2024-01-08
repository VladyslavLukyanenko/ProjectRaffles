using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PattaModule
{
  [RaffleModuleName("Patta NL")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class Patta : AccountBasedRaffleModuleBase<IPattaClient>
  {
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size SKU", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly DynamicValuesPickerField _instagramAccount = new DynamicValuesPickerField("instagramAccount", "Instagram", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };

    private readonly AddressFields _addressFields = new AddressFields
    {
      FullName = null,
      CountryId = null,
      AddressLine2 = null,
      ProvinceId = null
    };
    
    public Patta(IPattaClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https:\/\/www.patta\.nl\/blogs\/raffles\/.*")
    {
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _instagramAccount;
      yield return _addressFields;
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
      var parsedRaffle = await Client.ParseRaffleAsync(RaffleUrl, _sizeValue.Value, ct);

      Status = RaffleStatus.Submitting; 
      return await Client.SubmitAsync(_addressFields, SelectedAccount, parsedRaffle, _sizeValue.Value, RaffleUrl, _instagramAccount.Value, ct);
    }
  }
}