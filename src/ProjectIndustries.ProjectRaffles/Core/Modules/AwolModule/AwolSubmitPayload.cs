using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AwolModule
{
  public class AwolSubmitPayload
  {
    public AwolSubmitPayload(AddressFields profile, string email, string raffleurl, string sizevalue, string pickup, AwolParsedRaffle parsedRaffle)
    {
      Profile = profile;
      Email = email;
      RaffleUrl = raffleurl;
      SizeValue = sizevalue;
      PickupLocation = pickup;
      ParsedRaffle = parsedRaffle;
    }

    public AddressFields Profile { get; }
    public string Email { get; }
    public string RaffleUrl { get; }
    public string SizeValue { get; }
    public string PickupLocation { get; }
    public AwolParsedRaffle ParsedRaffle { get; }
  }
}