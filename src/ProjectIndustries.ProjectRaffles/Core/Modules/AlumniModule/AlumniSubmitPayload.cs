using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AlumniModule
{
  public class AlumniSubmitPayload
  {
    public AlumniSubmitPayload(Profile profile, string sizevalue, AlumniParsedRaffle parsedRaffle, string email)
    {
      Profile = profile;
      SizeValue = sizevalue;
      ParsedRaffle = parsedRaffle;
      Email = email;
    }

    public Profile Profile { get; }
    public string SizeValue { get; }
    public AlumniParsedRaffle ParsedRaffle { get; }
    public string Email { get; }
  }
}