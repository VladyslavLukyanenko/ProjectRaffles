using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.Raffle4ElementosModule
{
  public class Raffle4elementosSubmitPayload
  {
    public Raffle4elementosSubmitPayload(AddressFields profile, string email, string raffleurl, string size, string instahandle, Raffle4elementosParsed parsedRaffle)
    {
      Profile = profile;
      Email = email;
      RaffleUrl = raffleurl;
      SizeValue = size;
      InstaHandle = instahandle;
      ParsedRaffle = parsedRaffle;
    }

    public AddressFields Profile { get; }
    public string Email { get; }
    public string RaffleUrl { get; }
    public string SizeValue { get; }
    public string InstaHandle { get; }
    public Raffle4elementosParsed ParsedRaffle { get; }
  }
}