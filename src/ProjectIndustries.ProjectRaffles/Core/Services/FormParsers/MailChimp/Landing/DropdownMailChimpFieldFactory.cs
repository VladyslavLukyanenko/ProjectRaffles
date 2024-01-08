using System.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing
{
  public class DropdownMailChimpFieldFactory : MailChimpFieldFactoryBase<MailChimpDropdownField>
  {
    public DropdownMailChimpFieldFactory()
      : base("radio", "dropdown")
    {
    }

    protected override void SetSpecificFieldProps(SettingsField source, MailChimpDropdownField destination)
    {
      destination.Choices = source.Choices.ToList();
    }
  }
}