using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SvdModule
{
  public class SvdRaffleSubmitPayload
  {
    public SvdRaffleSubmitPayload(Profile profile, Account account, string raffleId, string optionId,
      string shippingMethod, string paymentMethod)
    {
      Profile = profile;
      Account = account;
      RaffleId = raffleId;
      OptionId = optionId;
      ShippingMethod = shippingMethod;
      PaymentMethod = paymentMethod;
    }

    public Profile Profile { get; }
    public Account Account { get; }
    public string RaffleId { get; }
    public string OptionId { get; }
    public string ShippingMethod { get; }
    public string PaymentMethod { get; }
  }
}