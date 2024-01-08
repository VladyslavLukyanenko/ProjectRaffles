using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SlamjamModule
{
  public class SlamjamSubmitPayload
  {
    public SlamjamSubmitPayload(Profile profile, string raffleurl, string variant, string store)
    {
      Profile = profile;
      RaffleUrl = raffleurl;
      Variant = variant;
      Store = store;
    }

    public Profile Profile { get; }
    public string RaffleUrl { get; }
    public string Variant { get; }
    public string Store { get; }
  }
}