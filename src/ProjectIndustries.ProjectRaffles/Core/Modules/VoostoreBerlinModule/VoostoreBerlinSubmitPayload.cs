using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.VoostoreBerlinModule
{
  public class VoostoreBerlinSubmitPayload
  {
    public VoostoreBerlinSubmitPayload(AddressFields profile, string email, string captcha, string size, string raffleurl,
      VoostoreBerlinRaffleTags raffletags, string housenumber)
    {
      Profile = profile;
      Email = email;
      RaffleUrl = raffleurl;
      Captcha = captcha;
      RaffleTags = raffletags;
      SizeValue = size;
      HouseNumber = housenumber;
    }

    public AddressFields Profile { get; }
    public string Email { get; }
    public VoostoreBerlinRaffleTags RaffleTags { get; }
    public string RaffleUrl { get; }
    public string Captcha { get; }
    public string SizeValue { get; }
    public string HouseNumber { get; }
  }
}