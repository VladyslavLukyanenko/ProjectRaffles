using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule
{
  [DisabledRaffleModule]
  [RaffleModuleName("Stay Rooted")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.Stripe)]
  [RaffleRegion(RaffleRegion.NorthAmerica)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class StayRooted : RaffleModuleBase
  {
    private readonly IStayRootedClient _client;

    private readonly TextField _urlId =
      new TextField("url", "Raffle URL", true, FieldValidators.IsMatches(@"https:\/\/stay-rooted\.com\/.*"));
    private readonly TextField _variantId = new TextField("variantId", "Variant ID", true);
    private readonly TextField _productId = new TextField("productId", "Product ID", true);
    private readonly DynamicValuesPickerField _email =
      new DynamicValuesPickerField("email", "Email", true, null, Pickers.All);


    public StayRooted(IStayRootedClient client)
    {
      _client = client;
    }

    public override IEnumerable<Field> AdditionalFields
    {
      get
      {
        yield return _urlId;
        yield return _variantId;
        yield return _productId;
        yield return _email;
      }
    }

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
      //todo: make statuses
      var validationPayload = new StayRootedValidationPayload(profile, _productId.Value, _email.Value);
      var customerid = await _client.GetCustomerIdAsync(validationPayload, ct);

      var entryPayload = new StayRootedEntryPayload(profile, _variantId.Value, customerid);
      var entryId = await _client.StartEntryProcessAsync(entryPayload, ct);

      //Todo: need to send stripe requests with creditcard, then parse the returned json and grab the payment token which looks like tok_blabla
      var checkoutToken = "";

      Status = RaffleStatus.Submitting;
      var finalPayload = new StayRootedFinalPayload(checkoutToken, entryId);
      Status = await _client.SubmitAsync(finalPayload, ct)
        ? RaffleStatus.Succeeded
        : RaffleStatus.Failed;
    }
  }
}