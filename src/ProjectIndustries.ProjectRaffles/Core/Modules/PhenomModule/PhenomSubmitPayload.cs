using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PhenomModule
{
  public class PhenomSubmitPayload
  {
    public PhenomSubmitPayload(AddressFields profile, string email, PhenomParsedRaffle parsedraffle, string raffleurl, string size,
      string instahandle)
    {
      Profile = profile;
      Email = email;
      RaffleUrl = raffleurl;
      SizeValue = size;
      ParsedRaffle = parsedraffle;
      InstagramHandle = instahandle;
    }

    public AddressFields Profile { get; }
    public string Email { get; }
    public string RaffleUrl { get; }
    public string SizeValue { get; }
    public PhenomParsedRaffle ParsedRaffle { get; }
    public string InstagramHandle { get; }
  }
}