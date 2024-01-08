using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SOTFModule
{
  public class SOTFSubmitPayload
  {
    public SOTFSubmitPayload(Profile profile, string colourway, string size, string instagram, string captcha,
      string formid, string email)
    {
      Profile = profile;
      Colourway = colourway;
      SizeValue = size;
      InstagramHandle = instagram;
      Captcha = captcha;
      FormId = formid;
      Email = email;
    }

    public Profile Profile { get; }
    public string Colourway { get; }
    public string SizeValue { get; }
    public string InstagramHandle { get; }
    public string Captcha { get; }
    public string FormId { get; }
    public string Email { get; }
  }
}