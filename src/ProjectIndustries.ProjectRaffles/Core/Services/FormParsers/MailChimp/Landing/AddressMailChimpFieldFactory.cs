using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing
{
  public class AddressMailChimpFieldFactory : MailChimpFieldFactoryBase<MailChimpAddressField>
  {
    public AddressMailChimpFieldFactory()
      : base("address")
    {
    }
  }
}