using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PanthersModule
{
  public class PanthersSubmitPayload
  {
    public PanthersSubmitPayload(AddressFields profile, Account account, PanthersParsedRaffle parsedraffle, string raffleurl, string size, string instahandle)
    {
      Profile = profile;
      Account = account;
      RaffleUrl = raffleurl;
      SizeValue = size;
      ParsedRaffle = parsedraffle;
      InstagramHandle = instahandle;
    }

    public AddressFields Profile { get; }
    public Account Account { get; }
    public string RaffleUrl { get; }
    public string SizeValue { get; }
    public PanthersParsedRaffle ParsedRaffle { get; }
    public string InstagramHandle { get; }
  }
}