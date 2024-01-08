using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SlamjamModule
{
  [DisabledRaffleModule]
  [RaffleModuleName("Slamjam")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class Slamjam : RaffleModuleBase
  {
    private readonly ISlamjamClient _client;

    private readonly TextField _variant = new TextField("variantId", "Variant", true);
    private readonly TextField _storeId = new TextField("storeId", "Store ID", true);
    private readonly TextField _raffleUrl = new TextField("raffleurl", "Raffle URL", true, FieldValidators.IsMatches(@"https:\/\/www\.slamjam\.com\/.*"));


    public Slamjam(ISlamjamClient client)
    {
      _client = client;
    }

    public override IEnumerable<Field> AdditionalFields
    {
      get
      {
        yield return _variant;
        yield return _storeId;
        yield return _raffleUrl;
      }
    }

    public List<AntiBotType> AntiBotTypes { get; } = new List<AntiBotType>
    {
      AntiBotType.Datadome
    };

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product
      {
        Name = "Fake Product",
        Picture = "/Assets/testProductPicture.png"
      });
    }
    

    protected override IModuleHttpClient HttpClient => _client;

    protected override async Task ExecuteAsync(Profile profile, CancellationToken ct)
    {
      var payload = new SlamjamSubmitPayload(profile, _raffleUrl.Value, _variant.Value, _storeId.Value);

      Status = RaffleStatus.Submitting;
      Status = await _client.SubmitAsync(payload, ct)
        ? RaffleStatus.Succeeded
        : RaffleStatus.Failed;
    }
  }
}