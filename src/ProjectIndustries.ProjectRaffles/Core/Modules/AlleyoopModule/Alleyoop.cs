using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AlleyoopModule
{
  [RaffleModuleName("Alleyoop JP")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Asia)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class Alleyoop : EmailBasedRaffleModuleBase<IAlleyoopClient>
  {
    private readonly TextField _friganaFirstName = new TextField(displayName: "Frigana first name");
    private readonly TextField _friganaLastName = new TextField(displayName: "Frigana last name");
    
    private readonly TextField _building = new TextField(displayName: "Building", isRequired: false);

    private readonly RadioButtonGroupField<string> _paymentMethod = new RadioButtonGroupField<string>("paymentMethod", 
      new Dictionary<string, string>
     {
     {"Credit Card", "10"}
     }, "Payment Method");
    
    private readonly OptionsField _deliveryTime = new OptionsField("deliveryTime", "Delivery time", true, new[]
    {
     "指定無し",
     "午前中", 
     "14時～16時",
     "16時～18時",
     "18時～20時",
     "19時～21時"
     });
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      FullName = null,
      CountryId = null,
      AddressLine1 = null,
      AddressLine2 = null
    };


    public Alleyoop(IAlleyoopClient client)
      : base(client, @"https:\/\/alleyoop\.shop\/form1\.php\?item_code=.*")
    {
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();

      yield return _friganaFirstName;
      yield return _friganaLastName;
      yield return _deliveryTime;
      yield return _paymentMethod;
      
      yield return _building;
      yield return _addressFields;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product {Name = "Alleyoop Raffle"});
    }
    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      var building = "";
      if (_building.Value != null)
      {
        building = _building.Value;
      }
      
      
      Status = RaffleStatus.GettingRaffleInfo;
      var parsedProduct = await Client.GetProductCode(RaffleUrl, ct);
      
      Status = RaffleStatus.Submitting;
      var payload = new AlleyoopSubmitPayload(RaffleUrl, _addressFields, EmailField, _paymentMethod.Value, _friganaFirstName.Value, _friganaLastName.Value, _deliveryTime.Value, parsedProduct, building);
      
      return await Client.SubmitAsync(payload, ct);
    }
  }
}