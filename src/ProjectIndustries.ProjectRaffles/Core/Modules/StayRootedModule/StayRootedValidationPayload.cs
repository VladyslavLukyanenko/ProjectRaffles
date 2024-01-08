using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule
{
  public class StayRootedValidationPayload
  {
    public StayRootedValidationPayload(Profile profile, string productid, string email)
    {
      Profile = profile;
      ProductId = productid;
      Email = email;
    }

    public Profile Profile { get; }
    public string ProductId { get; }
    public string Email { get; }
  }
}