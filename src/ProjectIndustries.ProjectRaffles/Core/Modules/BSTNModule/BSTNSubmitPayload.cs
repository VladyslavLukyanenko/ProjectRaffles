using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.BSTNModule
{
  public class BSTNSubmitPayload
  {
    public BSTNSubmitPayload(Profile profile, string captcha, string sizevalue, string instahandle, string raffleurl, string email)
    {
      Profile = profile;
      RaffleUrl = raffleurl;
      Email = email;
      SizeValue = sizevalue;
      InstaHandle = instahandle;
      Captcha = captcha;
      //BirthDay = bday;
      //BirthMonth = bmonth;
    }

    public Profile Profile { get; }
    public string RaffleUrl { get; }
    public string Email { get; }
    public string SizeValue { get; }
    public string InstaHandle { get; }
    public string Captcha { get; }
    //public string BirthDay { get; }
    //public string BirthMonth { get; }
  }
}