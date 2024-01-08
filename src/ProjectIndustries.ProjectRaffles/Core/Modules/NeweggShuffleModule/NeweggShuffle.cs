using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.NeweggAccountGenerator;
using ProjectIndustries.ProjectRaffles.Core.Modules.NakedCphModule;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NeweggShuffleModule
{
  [RaffleModuleName("Newegg Shuffle")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.NorthAmerica)]
  [RaffleReleaseType(RaffleReleaseType.RaffleFcfs)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  [RaffleAccountGenerator(typeof(NeweggAccountGenerator))]
  public class NeweggShuffle : AccountBasedRaffleModuleBase<INeweggShuffleClient>
  {
    private readonly ICaptchaSolveService _captchaSolver;

    private readonly DynamicValuesPickerField _shuffleId =
      new DynamicValuesPickerField("lotteryId", "Shuffle ID", true, null, Pickers.All)
      {
        // SelectedResolver = Pickers.Misc.ListItem
      };

    public NeweggShuffle(INeweggShuffleClient client, ICaptchaSolveService captchaSolver)
      : base(client, @".*")
    {
      _captchaSolver = captchaSolver;
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _shuffleId;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product {Name = "Newegg Shuffle, ID: " + _shuffleId.Value});
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.LoggingIntoAccount;
      if (SelectedAccount.AccessToken != null) //only works if generated inside bot, but I haven't experienced any errors with cookie based login
      {
        await Client.LoginWithCookies(SelectedAccount, ct);
      }
      else
      {
        var ticketId = await Client.GetTicketId(ct);
        var formData = await Client.ParseData(ticketId, ct);
        await Client.LoginWithEmail(SelectedAccount, formData, ticketId, ct); //fails sometimes for some reason, maybe parsed json needs to be edited?
      }

      var lotteryId = await Client.GetLotteryId(ct);

      var signupStatus = await Client.SignupToShuffle(_shuffleId.Value, lotteryId, ct);
      
      return signupStatus;
    }
  }
}