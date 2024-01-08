using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing
{
  public class TextMailChimpFieldFactory : MailChimpFieldFactoryBase<MailChimpTextField>
  {
    public TextMailChimpFieldFactory()
      : base("zip", "phone", "text", "email", "url", "imageurl", "number")
    {
    }
  }
}