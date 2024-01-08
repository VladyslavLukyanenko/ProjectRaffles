using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AlumniModule
{
  [DisabledRaffleModule] //they might have moved to Copdate
  [RaffleModuleName("Alumni")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.NorthAmerica)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class Alumni : RaffleModuleBase
  {
    private readonly IAlumniClient _client;
    private readonly SelectField<string> _size;
    private readonly DynamicValuesPickerField _email =
      new DynamicValuesPickerField("email", "Email", true, null, Pickers.All);


    public Alumni(IAlumniClient client)
    {
      _client = client;

      var sizes = new List<KeyValuePair<string, object>>();
      for (double i = 36; i <= 45; i += .5)
      {
        var num = i.ToString(CultureInfo.InvariantCulture);
        sizes.Add(new KeyValuePair<string, object>(num, num));
      }

      _size = new SelectField<string>("sizeValue", "Size", true, options: sizes.ToArray());
    }

    public override IEnumerable<Field> AdditionalFields
    {
      get
      {
        yield return _size;
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
      Status = RaffleStatus.GettingRaffleInfo;
      var parsedRaffle = await _client.ParseRaffleAsync(ct);

      var payload = new AlumniSubmitPayload(profile, _size.Value, parsedRaffle, _email.Value);
      Status = await _client.SubmitAsync(payload, ct)
        ? RaffleStatus.Succeeded
        : RaffleStatus.Failed;
    }
  }
}