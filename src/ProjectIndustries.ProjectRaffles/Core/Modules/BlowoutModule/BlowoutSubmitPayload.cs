using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.BlowoutModule
{
  public class BlowoutSubmitPayload
  {
    public BlowoutSubmitPayload(AddressFields profile, string email, string captcha, string raffleurl, string size, string instagramhandle,
      string pickup, BlowoutParsedRaffle parsedRaffle, string csrfToken, string housenumber)
    {
      Profile = profile;
      Email = email;
      RaffleUrl = raffleurl;
      SizeValue = size;
      Captcha = captcha;
      InstagramHandle = instagramhandle;
      PickUpLocation = pickup;
      ParsedRaffle = parsedRaffle;
      CsrfToken = csrfToken;
      HouseNumber = housenumber;
    }

    public AddressFields Profile { get; private set; }
    public string Email { get; }
    public string RaffleUrl { get; private set; }
    public string SizeValue { get; private set; }
    public string InstagramHandle { get; private set; }
    public string Captcha { get; private set; }
    public string PickUpLocation { get; private set; }
    public BlowoutParsedRaffle ParsedRaffle { get; private set; }
    public string CsrfToken { get; private set; }
    public string HouseNumber { get; private set; }
  }
}