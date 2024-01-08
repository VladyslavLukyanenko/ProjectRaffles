using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NakedCphModule
{
  public class NakedCphSubmitPayload
  {
    public NakedCphSubmitPayload(AddressFields profile, Account account, string captcha, string raffleurl, string ip, string instagram,
      string tags)
    {
      Profile = profile;
      Account = account;
      RaffleUrl = raffleurl;
      IP = ip;
      Captcha = captcha;
      ProductTags = tags;
      Instagram = instagram;
    }

    public AddressFields Profile { get; }
    public Account Account { get; }
    public string ProductTags { get; }
    public string RaffleUrl { get; }
    public string IP { get; }
    public string Captcha { get; }
    public string Instagram { get; }
  }
}