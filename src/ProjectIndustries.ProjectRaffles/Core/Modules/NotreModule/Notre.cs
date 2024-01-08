using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NotreModule
{
  [RaffleModuleName("Notre")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Custom)]
  [RafflePaymentProcessorType(PaymentProcessorType.None)]
  [RaffleRegion(RaffleRegion.NorthAmerica)]
  [RaffleReleaseType(RaffleReleaseType.Raffle)]
  [RaffleRequiresCaptcha(CaptchaRequirement.Required)]
  public class Notre : EmailBasedRaffleModuleBase<INotreClient>
  {
    private readonly TextField _raffleId = new TextField(displayName: "Raffle ID (all for all active)", isRequired: true);
    
    private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size (random for totally random)", true, null, Pickers.All)
    {
      // SelectedResolver = Pickers.Misc.ListItem
    };

    private readonly AddressFields _addressFields = new AddressFields
    {
      FullName = null,
      AddressLine2 = null
    };
    
    public Notre(INotreClient client, ICaptchaSolveService captchaSolver)
      : base(client, @".*")
    {
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return base.GetDeclaredFields();
      yield return _sizeValue;
      yield return _raffleId;
      yield return _addressFields;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product {Name = "Notre Raffle"});
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken ct)
    {
      Status = RaffleStatus.GettingRaffleInfo;
      var parsedRaffle = await Client.ParseNotreAsync(ct);

      await Client.SetXsrf(ct);

      Status = RaffleStatus.SolvingCAPTCHA;
      var notreQuestion = await Client.GetCaptchaQuestionAsync(ct);

      var captchaAnswer = await Client.GetCaptchaAnswerAsync(notreQuestion.Question, ct);

      var validatedAnswer = await Client.ValidateAnswerAsync(notreQuestion, captchaAnswer);

      while (!validatedAnswer.IsValidAnswer)
      {
        notreQuestion = await Client.GetCaptchaQuestionAsync(ct);
        captchaAnswer = await Client.GetCaptchaAnswerAsync(notreQuestion.Question, ct);
        validatedAnswer = await Client.ValidateAnswerAsync(notreQuestion, captchaAnswer);
      }

      await Client.RegisterUser(_addressFields, validatedAnswer, EmailField, ct);

      Status = RaffleStatus.Submitting;
      if (_raffleId.Value.ToLower() == "all")
      {
        foreach (var raffleId in parsedRaffle.RaffleIds)
        {
          await Client.SubmitEntryToRaffleAsync(parsedRaffle, raffleId, _sizeValue.Value, ct);
        }

        return true; //will only do this if none of the raffles have errored :)
      }
      else
      {
        int raffleIdConverted = Convert.ToInt32(_raffleId.Value);
        return await Client.SubmitEntryToRaffleAsync(parsedRaffle, raffleIdConverted, _sizeValue.Value, ct);
      }

    }
  }
}