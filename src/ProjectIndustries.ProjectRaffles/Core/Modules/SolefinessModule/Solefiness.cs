using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SolefinessModule
{
  [RaffleModuleName("Solefiness")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.MailChimp)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Oceania)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class Solefiness : EmailBasedRaffleModuleBase<ISolefinessClient>
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
      FullName = null
    };
    
    private readonly CreditCardFields _creditCard = new CreditCardFields();

    public Solefiness(ISolefinessClient client)
      : base(client, @"https:\/\/www\.solefiness\.com\/.*")
    {
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _instagramHandle;
      yield return _sizeValue;
      yield return _addressFields;
      yield return _creditCard;
    }

    protected override async Task<Product> FetchProductAsync(CancellationToken ct)
    {
      var product = await Client.GetProductAsync(RaffleUrl, ct);
      return new Product {Name = product};
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parsedRaffle = await Client.ParseRaffleAsync(RaffleUrl, ct);
      
      Status = RaffleStatus.Submitting;
      var jsonContent = await Client.CraftContentAsync(_addressFields, _creditCard, parsedRaffle, EmailField, _instagramHandle.Value, _sizeValue.Value);
      return await Client.SubmitAsync(parsedRaffle, jsonContent, ct);
    }
  }
}