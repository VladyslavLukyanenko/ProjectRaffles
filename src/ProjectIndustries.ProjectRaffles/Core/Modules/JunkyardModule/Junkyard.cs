using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.JunkyardModule
{
  [RaffleModuleName("Junkyard")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  public class Junkyard : EmailBasedRaffleModuleBase<IJunkyardClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    private readonly IStringUtils _stringUtils;

    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };
    
    private readonly RadioButtonGroupField<bool> _captchaBypass = new RadioButtonGroupField<bool>("Captcha bypass", new Dictionary<string, bool>
    {
      {"Yes", true},
      {"No", false}
    }, "Captcha Bypass");

    public Junkyard(IJunkyardClient client, ICaptchaSolveService captchaSolver, IStringUtils stringUtils)
      : base(client, @"https:\/\/www\.junkyard\..{1,3}\/junkraffle\/.*")
    {
      _captchaSolver = captchaSolver;
      _stringUtils = stringUtils;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
    }

    protected override async Task<Product> FetchProductAsync(CancellationToken ct)
    {
      var product = await Client.GetProductAsync(RaffleUrl, ct);
      return new Product {Name = product};
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parseRaffle = await Client.ParseRaffleAsync(_sizeValue.Value, RaffleUrl, ct);


      string captcha;
      Status = RaffleStatus.SolvingCAPTCHA;
      if (_captchaBypass.Value)
      {
        var generatedCaptchaLetters = await _stringUtils.GenerateRandomStringAsync(21);
        captcha =
          @"03AGdBq27TRv-En2V8dECxmZ5Byl31m60vytbPIdvdz7Thok3XR8gZKZBrGjNlTw5vcpPYA3btUhy9N8cjbMhz67o9m9UHfLGqYGsl9TyjhqjPhAWlSdyL0D9QfsYvw2MZibaoBqAFpdfUY5F67Z6e0iRxLHqOspC69gQYiIPMiGU7226aF6hY5UeeaQJjcxMTuYtowEZ2lXLbP6SFSW9TCOSGxd5GLm0IMp_vKzgyB4JeJdshQRS63pStA1sHRPOkMFLe81xq3bH2n4-1a30q3Q9mxFL_F307tfuqKrCBSEnaJUvRQ69fQnwG18fXbu3rYTkH1VYBivxpSD0PylRiMe4Q9AVpyACQfF8eOdD6hKldoMA8UQk95FSLrnG2197ks-srL6dsmbu7OTRlOY-Us2lAGWYr5KUImSpHcv952BOZSYCBc1NxglQz9z3ZD1lgPLuxKEGeZ46_" + generatedCaptchaLetters;
      }
      else
      {
        captcha =
          await _captchaSolver.SolveReCaptchaV2Async("6LcFPiYUAAAAAHJmKRSK5V3eHY8DOhP0tQCk1Ll2", RaffleUrl, true,
            ct);
      }

      Status = RaffleStatus.Submitting;
      var payload = new JunkyardSubmitPayload(EmailField, captcha, RaffleUrl, parseRaffle);
      return await Client.SubmitAsync(payload, ct);
    }
  }
}