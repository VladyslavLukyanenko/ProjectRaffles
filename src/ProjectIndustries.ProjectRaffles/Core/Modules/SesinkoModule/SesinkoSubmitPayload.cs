using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SesinkoModule
{
  public class SesinkoSubmitPayload
  {
    public SesinkoSubmitPayload(AddressFields profile, string email, SesinkoParsedRaffle parsedraffle, string raffleurl, string size,
      string instahandle, string pickup)
    {
      Profile = profile;
      RaffleUrl = raffleurl;
      SizeValue = size;
      ParsedRaffle = parsedraffle;
      InstagramHandle = instahandle;
      PickupLocation = pickup;
    }

    public AddressFields Profile { get; }
    public string Email { get; }
    public string RaffleUrl { get; }
    public string SizeValue { get; }
    public SesinkoParsedRaffle ParsedRaffle { get; }
    public string InstagramHandle { get; }
    public string PickupLocation { get; }
  }
}