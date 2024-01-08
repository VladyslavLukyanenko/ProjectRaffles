using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketLondonModule
{
  [RaffleModuleName("DSM London")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Formstack)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe | RaffleRegion.NorthAmerica)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class DoverStreetMarketLondon : EmailBasedRaffleModuleBase<IDoverStreetMarketLondonClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    private readonly IStringUtils _stringUtils;
    
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly TextField _variant = new TextField(displayName: "Variant", isRequired: false);
    
    private readonly TextField _question = new TextField(displayName: "Question", isRequired: false);
    private readonly DynamicValuesPickerField _questionAnswer = new DynamicValuesPickerField("answer", "Answer", false, null, Pickers.All)
    {
    };
    
    private readonly RadioButtonGroupField<bool> _parseHidden = new RadioButtonGroupField<bool>("hiddenFields",
     new Dictionary<string, bool>
     {
     {"Yes", true},
     {"No", false},
     }, "Parse multiple sizes?");
    
    private readonly TextField _shipping = new TextField(displayName: "Shipping Option", isRequired:false);
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      FirstName = null,
      LastName = null,
      ProvinceId = null,
      AddressLine2 = null
    };

    public DoverStreetMarketLondon(IDoverStreetMarketLondonClient client, ICaptchaSolveService captchaSolver, IStringUtils stringUtils)
      : base(client, @"https:\/\/london\.doverstreetmarket\.com\/new-items\/.*")
    {
      _captchaSolver = captchaSolver;
      _stringUtils = stringUtils;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _addressFields;
      yield return _variant;
      yield return _parseHidden;
      yield return _question;
      yield return _questionAnswer;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      
      return Task.FromResult(new Product
      {
        Name = "DSML Raffle"
      });
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
       Status = RaffleStatus.GettingRaffleInfo;
      var parseRaffle = await Client.ParseRaffleAsync(RaffleUrl, _variant.Value, _question.Value, _parseHidden.Value, ct);

      var nonce = await _stringUtils.GenerateRandomStringAsync(16);


      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha =
        await _captchaSolver.SolveReCaptchaV2Async("6LetKEIUAAAAAPk-uUXqq9E82MG3e40OMt_74gjS", RaffleUrl, true,
          ct);

      Status = RaffleStatus.Submitting;
      var payload =
        new DoverStreetMarketLondonSubmitPayload(_addressFields, EmailField, parseRaffle, nonce, captcha, _sizeValue.Value, _variant.Value, _questionAnswer.Value, _shipping.Value);

      return await Client.SubmitAsync(payload, ct);
    }
  }
}