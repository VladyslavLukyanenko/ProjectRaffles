using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DelibertiModule
{
  [RaffleModuleName("Deliberti")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class Deliberti : EmailBasedRaffleModuleBase<IDelibertiClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly DynamicValuesPickerField _instagramHandle = new DynamicValuesPickerField("instagramHandle", "Instagram", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly TextField _region = new TextField(displayName: "Region/Province - needed even if country doesn't have");
    private readonly TextField _streetNo = new TextField(displayName: "House Number");
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      ProvinceId = null
    };

    public Deliberti(IDelibertiClient client, ICaptchaSolveService solveService)
      : base(client, @"https:\/\/deliberti\.it\/.*")
    {
      _captchaSolver = solveService;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _instagramHandle;
      yield return _region;
      yield return _streetNo;
      yield return _addressFields;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      var product = "Deliberti raffle";
      return Task.FromResult(new Product {Name = product});
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    { 
      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(_addressFields, EmailField, _sizeValue.Value, _streetNo.Value, _region.Value,
        _instagramHandle.Value, RaffleUrl, ct);
    }
  }
}