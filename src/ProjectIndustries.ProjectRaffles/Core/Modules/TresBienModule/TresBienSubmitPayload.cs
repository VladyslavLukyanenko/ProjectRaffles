using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TresBienModule
{
  public class TresBienSubmitPayload
  {
    public TresBienSubmitPayload(AddressFields profile,string email, string captcha, string size, string raffleurl,
      TresBienProductTags product)
    {
      Profile = profile;
      Email = email;
      RaffleUrl = raffleurl;
      Captcha = captcha;
      ProductTags = product;
      SizeValue = size;
    }

    public AddressFields Profile { get; }
    public string Email { get; }
    public TresBienProductTags ProductTags { get; }
    public string RaffleUrl { get; }
    public string Captcha { get; }
    public string SizeValue { get; }
  }
}