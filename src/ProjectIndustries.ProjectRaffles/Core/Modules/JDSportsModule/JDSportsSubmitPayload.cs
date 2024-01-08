using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.JDSportsModule
{
  public class JDSportsSubmitPayload
  {
    public JDSportsSubmitPayload(Profile profile, string captcha, string sizevalue, string raffleurl, string store, string baseurl, string county, string email, string paypalemail)
    {
      Profile = profile;
      RaffleUrl = raffleurl;
      SizeValue = sizevalue;
      Captcha = captcha;
      Store = store;
      BaseUrl = baseurl;
      County = county;
      Email = email;
      Paypalemail = paypalemail;
    }

    public Profile Profile { get; }
    public string RaffleUrl { get; }
    public string SizeValue { get; }
    public string Captcha { get; }
    public string Store { get; }
    public string BaseUrl { get; }
    public string County { get; }
    public string Email { get; }
    public string Paypalemail { get; }
  }
}