using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.SnkrKuwaitAccountGenerator;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SnkrKuwaitModule
{
  [RaffleModuleName("Snkr Kuwait")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Asia)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleAccountGenerator(typeof(SnkrKuwaitAccountGenerator))]
  public class SnkrKuwait : AccountBasedRaffleModuleBase<ISnkrKuwaitClient>
  {
    private readonly DynamicValuesPickerField _size = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    

    private readonly AddressFields _addressFields = new AddressFields();

    public SnkrKuwait(ISnkrKuwaitClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https:\/\/www\.snkr\.com\.kw\/.*")
    {
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _size;
      yield return _addressFields;
    }

    protected override async Task<Product> FetchProductAsync(CancellationToken ct)
    {
      var product = await Client.GetRaffleProduct(RaffleUrl, ct);
      return new Product {Name = product};
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parsed = await Client.ParseRaffleAsync(RaffleUrl, ct);

      Status = RaffleStatus.Submitting;
      return await Client.SubmitAsync(_addressFields, parsed, SelectedAccount, _size.Value, RaffleUrl, ct);
    }
  }
}