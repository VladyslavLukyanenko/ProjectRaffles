using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NakedCphModule
{
  [RaffleModuleName("NakedCPH")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.Europe)]
  [RaffleReleaseType(RaffleReleaseType.RaffleFcfs)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class NakedCph : AccountBasedRaffleModuleBase<INakedCphClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;
    
    private readonly TextField _raffleTag = new TextField(displayName: "Raffle tag");

    private readonly AddressFields _addressFields = new AddressFields();
    
    private readonly DynamicValuesPickerField _instagramHandle = new DynamicValuesPickerField("instagramHandle", "Instagram", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };

    public NakedCph(INakedCphClient client, ICaptchaSolveService captchaSolver)
      : base(client, @"https:\/\/www\.nakedcph\.com\/.*")
    {
      _captchaSolver = captchaSolver;
    }

    private string GetSanitizedUrlWithoutQuery()
    {
      var uri = new Uri(RaffleUrl);
      var uriBuilder = new UriBuilder(uri.Scheme, uri.Host, uri.Port, uri.AbsolutePath);
      return uriBuilder.Uri.ToString();
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _raffleTag;
      yield return _instagramHandle;
      yield return _addressFields;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      //var product = await Client.GetProductAsync(GetSanitizedUrlWithoutQuery(), ct);
      return Task.FromResult(new Product {Name = "NakedCPH Raffle"});
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var ip = await Client.GetIPAsync();

//      var raffleDetails = await Client.GetRaffleTags(GetSanitizedUrlWithoutQuery(), ct);

      Status = RaffleStatus.SolvingCAPTCHA;
      var captcha =
        await _captchaSolver.SolveReCaptchaV2Async("6LfbPnAUAAAAACqfb_YCtJi7RY0WkK-1T4b9cUO8", RaffleUrl, true,
          ct);

      Status = RaffleStatus.Submitting;
      var payload = new NakedCphSubmitPayload(_addressFields, SelectedAccount, captcha, RaffleUrl, ip, _instagramHandle.Value, _raffleTag.Value);
      return await Client.SubmitAsync(payload, ct);
    }
  }
}