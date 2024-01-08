using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.WoeiAccountGenerator;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WoeiModule
{
  [RaffleModuleName("Woei")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleAccountGenerator(typeof(WoeiAccountGenerator))]
  public class Woei : AccountBasedRaffleModuleBase<IWoeiClient>
  {
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly DynamicValuesPickerField _instagramHandle = new DynamicValuesPickerField("instagramHandle", "Instagram", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly RadioButtonGroupField<string> _gender = new RadioButtonGroupField<string>("_gender",
      new Dictionary<string, string>
      {
        {"Male", "m"},
        {"Female", "f"},
      }, "Gender");
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      FullName = null,
      AddressLine2 = null,
      ProvinceId = null
    };
    public Woei(IWoeiClient client)
      : base(client, @"https:\/\/www\.woei-webshop\.nl\/.*")
    {
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _gender;
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
      var parsed = await Client.ParseProductAsync(RaffleUrl, ct);

      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(_addressFields, parsed, SelectedAccount, _instagramHandle.Value, _sizeValue.Value, _gender.Value, ct);
    }
  }
}