using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule
{
  public class StayRootedEntryPayload
  {
    public StayRootedEntryPayload(Profile profile, string variant, string customerid)
    {
      Profile = profile;
      VariantId = variant;
      CustomerId = customerid;
    }

    public Profile Profile { get; }
    public string VariantId { get; }
    public string CustomerId { get; }
  }
}