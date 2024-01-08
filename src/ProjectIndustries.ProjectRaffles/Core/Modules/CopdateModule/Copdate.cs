using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.CopdateModule
{
  [RaffleModuleName("Copdate")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.NorthAmerica)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class Copdate : AccountBasedRaffleModuleBase<ICopdateClient>
  {
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };

    private readonly RadioButtonGroupField<string> _gender = new RadioButtonGroupField<string>("gender",
      new Dictionary<string, string>
      {
        {"Male", "male"},
        {"Female", "female"},
      }, "Gender");

    private readonly AddressFields _addressFields = new AddressFields
    {
      AddressLine1 = null,
      AddressLine2 = null,
      ProvinceId = null,
      PostCode = null,
      City = null,
      CountryId = null
    };

    public Copdate(ICopdateClient client)
      : base(client, @"https:\/\/list\.copdate\.com/event/.*")
    {
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _addressFields;
    }

    protected override async Task<Product> FetchProductAsync(CancellationToken ct)
    {
      var product = await Client.GetProductAsync(RaffleUrl, ct);
      return new Product {Name = product};
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.VerifyingAccount;
      await Client.VerifyAccountAsync(SelectedAccount, _addressFields, ct);

      Status = RaffleStatus.GettingRaffleInfo;
      var parsedProduct = await Client.ParseRaffleAsync(RaffleUrl, ct);

      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(parsedProduct, SelectedAccount, _addressFields, _sizeValue.Value, _gender.Value,
        RaffleUrl, ct);
    }
  }
}