using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AmigoSkateshopModule
{
  [RaffleModuleName("Amigo Skateshop")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class AmigoSkateshop : EmailBasedRaffleModuleBase<IAmigoSkateshopClient>
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
    
    private readonly TextField _shipping = new TextField(displayName: "Shipping Type - Online or Instore");
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      ProvinceId = null
    };


    public AmigoSkateshop(IAmigoSkateshopClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https:\/\/amigoskateshop\.com\/pages\/.*")
    {
      _captchaSolver = captchaSolver;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _instagramHandle;
      yield return _shipping;

      yield return _addressFields;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      //var product =  _client.GetProductAsync(_raffleUrl.Value, ct);
      var product = RaffleUrl.Value.Replace("https://amigoskateshop.com/pages/", "");
      return Task.FromResult(new Product {Name = product});
    }
    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parsedProduct = await Client.ParseFormAsync(RaffleUrl, ct);

      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha = await _captchaSolver.SolveReCaptchaV2Async("6LehE7gZAAAAAKqJHatjfVZoLKHH_3x4bFybPes3",RaffleUrl, true, ct);

      Status = RaffleStatus.Submitting;
      var payload = new AmigoSkateshopSubmitPayload(_addressFields, EmailField, parsedProduct, _sizeValue.Value, captcha, _instagramHandle.Value, _shipping.Value, RaffleUrl);
      
      return await Client.SubmitAsync(payload, ct);
    }
  }
}