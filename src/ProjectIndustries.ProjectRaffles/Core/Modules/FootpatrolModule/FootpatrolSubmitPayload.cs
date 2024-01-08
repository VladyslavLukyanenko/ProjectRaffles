using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.FootpatrolModule
{
  public class FootpatrolSubmitPayload
  {
    public FootpatrolSubmitPayload(Profile profile, string captcha, string sizevalue, string raffleurl, string county, string email, string paypalEmail)
    {
      Profile = profile;
      RaffleUrl = raffleurl;
      SizeValue = sizevalue;
      Captcha = captcha;
      County = county;
      Email = email;
      PaypalEmail = paypalEmail;
    }

    public Profile Profile { get; }
    public string RaffleUrl { get; }
    public string SizeValue { get; }
    public string Captcha { get; }
    public string County { get; }
    public string Email { get; }
    public string PaypalEmail { get; }
  }
}