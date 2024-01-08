using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Apm;
using Elastic.Apm.Api;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker.AddressPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SvdModule
{
#if !DEBUG
  [DisabledRaffleModule]
#endif
  [RaffleModuleName("SVD")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.BrainTree)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  [RafflePaymentMethod(PaymentMethod.CreditCard)]
  [MailRaffleStatusExtractor(typeof(SvdMailRaffleStatusExtractor))]
  [MailRaffleConfirmationHandler(typeof(SvdMailRaffleConfirmationHandler))]
  [RaffleAccountGenerator(typeof(SvdAccountsGenerator))]
  public class Svd : AccountBasedRaffleModuleBase<ISvdClient>, IRaffleResultsEmailReceiver
  {
    // private readonly AccountPickerField _accountField =
    // new AccountPickerField("participationAccount", "Participation Account", true);

    private readonly EmailPickerField _email = new EmailPickerField("email", "Email");
    // private readonly TextField _raffleId = new TextField("raffleId", "Raffle", true);
    // private readonly CustomListPickerField _list = new CustomListPickerField("list", "List", true);
    // private readonly TextField _productId = new TextField("productId", "Product", true);
    // private readonly CheckboxField _autoJig = new CheckboxField("autoJig", "Auto Jig Profiles");

    // private readonly RadioButtonGroupField<string> _demoCase1 = new RadioButtonGroupField<string>("_demo",
    // new Dictionary<string, string>
    // {
    // {"Male", "VALUE_1"},
    // {"Female", "VALUE_2"},
    // }, "Submit gender");

    // private readonly TextField _optionId =
    // new TextField("optionId", "Option", true, s => Task.FromResult(s.StartsWith("VALID")));

    // private readonly DynamicValuesPickerField _dynamicPickerField1 = new DynamicValuesPickerField("_dynamicPickerField1",
    //   "Dynamic Field 1", false, null, Pickers.All)
    // {
    //   SelectedResolver = Pickers.ProfileFields.FirstName
    // };
    // private readonly DynamicValuesPickerField _dynamicPickerField2 = new DynamicValuesPickerField("_dynamicPickerField2",
    //   "Dynamic Field 2", false, null, Pickers.All)
    // {
    //   SelectedResolver = Pickers.ProfileFields.LastName
    // };

    // private readonly OptionsField _sizes = new OptionsField("sizes", "Sizes", true, new[]
    // {
    // "35-5 RU | UK 3-5",
    // "38-5 RU | UK 6-5",
    // "40-5 RU | UK 8",
    // "42-5 RU | UK 9",
    // "45-5 RU | UK 11-5",
    // });

    private readonly ITestService _testService;

    private readonly ISvdClient _svdClient;
    private readonly ITracer _tracer;

    private readonly DynamicValuesPickerField _addressLine1;
    private readonly DynamicValuesPickerField _addressLine2;
    private readonly DynamicValuesPickerField _houseNum;
    private readonly DynamicValuesPickerField _state;
    private readonly DynamicValuesPickerField _countryCode;
    private readonly DynamicValuesPickerField _postCode;

    public Svd(ISvdClient svdClient, ITestService testService, ITracer tracer)
      : base(svdClient, ".*", skipFieldsInitialization: true)
    {
      _svdClient = svdClient;
      _testService = testService;
      _tracer = tracer;
      var shippingAddressGroup = AddressPickerValuesGroup.CreateOptimized();

      _addressLine1 = new DynamicValuesPickerField("_addressLine1", "addressLine1", true, groups: shippingAddressGroup);
      _addressLine2 = new DynamicValuesPickerField("_addressLine2", "addressLine2", true, groups: shippingAddressGroup);
      _houseNum = new DynamicValuesPickerField("_houseNum", "houseNum", true, groups: shippingAddressGroup);
      _state = new DynamicValuesPickerField("_state", "state", true, groups: shippingAddressGroup);
      _countryCode = new DynamicValuesPickerField("_countryCode", "countryCode", true, groups: shippingAddressGroup);
      _postCode = new DynamicValuesPickerField("_postCode", "postCode", true, groups: shippingAddressGroup);
      _ = InitializeDefaultIdentityFields();
    }

    // public override IEnumerable<Field> AdditionalFields
    // {
    //   get
    //   {
    //     yield return _email;
    //     // yield return _accountField;
    //     // yield return _list;
    //     // yield return _raffleId;
    //     // yield return _productId;
    //     // yield return _optionId;
    //     // yield return _autoJig;
    //     // yield return _demoCase1;
    //     yield return _dynamicPickerField1;
    //     yield return _dynamicPickerField2;
    //     // yield return _sizes;
    //     // yield return _addressLine1;
    //     // yield return _addressLine2;
    //     // yield return _houseNum;
    //     // yield return _state;
    //     // yield return _countryCode;
    //     // yield return _postCode;
    //   }
    // }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _email;
      // yield return _accountField;
      // yield return _list;
      // yield return _raffleId;
      // yield return _productId;
      // yield return _optionId;
      // yield return _autoJig;
      // yield return _demoCase1;
      yield return _addressLine1;
      yield return _addressLine2;
      yield return _houseNum;
      yield return _state;
      yield return _countryCode;
      yield return _postCode;
      // yield return _dynamicPickerField1;
      // yield return _dynamicPickerField2;
    }

    protected override async Task<Product> FetchProductAsync(CancellationToken ct)
    {
      await Task.Delay(TimeSpan.FromMilliseconds(1000), ct);

      return new Product
      {
        Name = string.Join(", ", AdditionalFields.Select(_ => _.Value)),
        Picture = "https://images-na.ssl-images-amazon.com/images/I/41Leu3gBUFL.jpg"
      };
    }

    protected override IModuleHttpClient HttpClient => _svdClient;

    // protected override async Task ExecuteAsync(Profile profile, CancellationToken ct)
    // {
    //   var rnd = new Random((int) DateTime.Now.Ticks);
    //   Status = SvdStatus.FetchingShippingMethod;
    //   await Task.Delay(TimeSpan.FromSeconds(rnd.Next(5, 10)), ct);
    //
    //   Status = SvdStatus.ObtainingAuthorizationFingerprint;
    //   await Task.Delay(TimeSpan.FromSeconds(rnd.Next(10, 15)), ct);
    //
    //   var result = await _testService.CallResultWillBeCached("SomeUserName", 321);
    //   Status = SvdStatus.Demo(1 + result.ToString());
    //
    //   await Task.Delay(TimeSpan.FromSeconds(2), ct);
    //   result = await _testService.CallResultWillBeCached("SomeUserName", 321);
    //   Status = SvdStatus.Demo(2 + result.ToString());
    //
    //   await Task.Delay(TimeSpan.FromSeconds(2), ct);
    //   result = await _testService.CallResultWillBeCached("SomeUserName", 321);
    //   Status = SvdStatus.Demo(3 + result.ToString());
    //   await Task.Delay(TimeSpan.FromSeconds(2), ct);
    //
    //   Status = SvdStatus.Submitting;
    //   await Task.Delay(TimeSpan.FromSeconds(rnd.Next(10, 15)), ct);
    //   Status = DateTime.Now.Ticks % 2 == 0
    //     ? RaffleStatus.Succeeded
    //     : RaffleStatus.Failed;
    //
    //   // var acc = _accountField.GetNextAccount();
    //   //
    //   // Status = SvdStatus.FetchingShippingMethod;
    //   // var shippingMethod = await _svdClient.GetShippingMethodAsync(profile, acc, _raffleId.Value, ct);
    //   //
    //   // // Get authorization fingerprint
    //   // // what would we do with this fingerprint?
    //   //
    //   // Status = SvdStatus.ObtainingAuthorizationFingerprint;
    //   // var authorizationFingerprint = await _svdClient.GetAuthorizationFingerprintAsync(acc, ct);
    //   //
    //   //
    //   // string paymentMethod = null; // where do we get it?
    //   // //after payment
    //   // var payload =
    //   //   new SvdRaffleSubmitPayload(profile, acc, _raffleId.Value, _optionId.Value, shippingMethod, paymentMethod);
    //   //
    //   // Status = SvdStatus.Submitting;
    //   // var isSuccess = await _svdClient.SubmitRaffleAsync(payload, ct);
    //   //
    //   // Status = isSuccess
    //   //   ? RaffleStatus.Succeeded
    //   //   : RaffleStatus.Failed;
    // }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      var rnd = new Random((int) DateTime.Now.Ticks);
      Status = SvdStatus.FetchingShippingMethod;
      await Task.Delay(TimeSpan.FromSeconds(rnd.Next(5, 10)), ct);

      Status = SvdStatus.ObtainingAuthorizationFingerprint;
      await Task.Delay(TimeSpan.FromSeconds(rnd.Next(10, 15)), ct);

      var result = await _testService.CallResultWillBeCached("SomeUserName", 321);
      Status = SvdStatus.Demo(1 + result.ToString());

      await Task.Delay(TimeSpan.FromSeconds(2), ct);
      result = await _testService.CallResultWillBeCached("SomeUserName", 321);
      Status = SvdStatus.Demo(2 + result.ToString());

      await Task.Delay(TimeSpan.FromSeconds(2), ct);
      result = await _testService.CallResultWillBeCached("SomeUserName", 321);
      Status = SvdStatus.Demo(3 + result.ToString());
      await Task.Delay(TimeSpan.FromSeconds(2), ct);

      Status = SvdStatus.Submitting;
      await Task.Delay(TimeSpan.FromSeconds(rnd.Next(10, 15)), ct);
      return DateTime.Now.Ticks % 2 == 0;
    }

    public Email ReceiverEmail => _email.MaterializedValue;
  }
}