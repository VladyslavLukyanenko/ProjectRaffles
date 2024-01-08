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

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketNewYorkModule
{
  [RaffleModuleName("DSM New York")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Formstack)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe | RaffleRegion.NorthAmerica)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class DoverStreetMarketNewYork : EmailBasedRaffleModuleBase<IDoverStreetMarketNewYorkClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    private readonly IStringUtils _stringUtils;
    
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly TextField _variant = new TextField(displayName: "Variant", isRequired: false);
    
    private readonly RadioButtonGroupField<string> _mailingList = new RadioButtonGroupField<string>("mailingList",
     new Dictionary<string, string>
     {
     {"Yes", "Y"},
     {"No", "N"},
     }, "Has multiple mailing list fields?", false);
    
    private readonly TextField _question = new TextField(displayName: "Question", isRequired: false);
    private readonly DynamicValuesPickerField _questionAnswer = new DynamicValuesPickerField("answer", "Answer", false, null, Pickers.All)
    {
    };
    
    private readonly AddressFields _addressFields = new AddressFields
    {
      AddressLine2 = null,
      FirstName = null,
      LastName = null
    };
    
    private readonly RadioButtonGroupField<string> _parseType = new RadioButtonGroupField<string>("parseType",
     new Dictionary<string, string>
     {
     {"NY", "NY"},
     {"LDN", "LDN"},
   }, "Parse Type", false);

    public DoverStreetMarketNewYork(IDoverStreetMarketNewYorkClient client, ICaptchaSolveService captchaSolver, IStringUtils stringUtils)
      : base(client, @"https:\/\/newyork\.doverstreetmarket\.com\/new-items\/.*")
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
      yield return _parseType;
      yield return _mailingList;
      yield return _question;
      yield return _questionAnswer;
    }
    
    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product
      {
        Name = "DSMNY Raffle"
      });
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parseRaffle = await Client.ParseRaffleAsync(RaffleUrl, _variant.Value, _parseType.Value, _question.Value, ct);

      var nonce = await _stringUtils.GenerateRandomStringAsync(16);


      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha =
        await _captchaSolver.SolveReCaptchaV2Async("6LetKEIUAAAAAPk-uUXqq9E82MG3e40OMt_74gjS", RaffleUrl, true,
          ct);

      Status = RaffleStatus.Submitting;
      var payload =
        new DoverStreetMarketNewYorkSubmitPayload(_addressFields, EmailField, parseRaffle, nonce, captcha, _sizeValue.Value, _variant.Value, _questionAnswer.Value, _mailingList.Value);

      return await Client.SubmitAsync(payload, ct);
    }
  }
}