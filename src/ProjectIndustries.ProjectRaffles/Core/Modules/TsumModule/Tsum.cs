using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TsumModule
{
  [RaffleModuleName("TSUM")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.MailChimp)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class Tsum : EmailBasedRaffleModuleBase<ITsumClient>
  {
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly TextField _age = new TextField(displayName: "Age");

    private readonly RadioButtonGroupField<string> _pickupLocation = new RadioButtonGroupField<string>("pickup",
      new Dictionary<string, string>
      {
        {"TSUM", "TSUM"},
        {"DLT", "DLT"},
      }, "Pickup location");
    
    private readonly RadioButtonGroupField<string> _gender = new RadioButtonGroupField<string>("gender",
      new Dictionary<string, string>
      {
        {"Female", "0"},
        {"Male", "1"},
      }, "Gender");
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      FullName = null,
      AddressLine1 = null,
      AddressLine2 = null,
      ProvinceId = null,
      City = null,
      PostCode = null,
      CountryId = null
    };


    public Tsum(ITsumClient client)
      : base(client, @"https:\/\/www.tsum\.ru\/lp\/.*/")
    {
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _age;
      yield return _gender;
      yield return _pickupLocation;
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
      var parsedProduct = await Client.ParseRaffleAsync(RaffleUrl, ct);

      Status = RaffleStatus.Submitting;
      var payload =
        new TsumSubmitPayload(_addressFields, EmailField, parsedProduct, _pickupLocation.Value, _sizeValue.Value, _gender.Value, _age.Value);
      return await Client.SubmitAsync(payload, ct);
    }
  }
}