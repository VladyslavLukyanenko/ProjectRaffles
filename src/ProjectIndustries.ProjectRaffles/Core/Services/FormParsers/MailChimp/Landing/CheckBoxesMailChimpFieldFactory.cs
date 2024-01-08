using System.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing
{
  public class CheckBoxesMailChimpFieldFactory : MailChimpFieldFactoryBase<MailChimpCheckBoxesField>
  {
    public CheckBoxesMailChimpFieldFactory()
      : base("checkbox")
    {
    }

    protected override void SetSpecificFieldProps(SettingsField source, MailChimpCheckBoxesField destination)
    {
      destination.Choices = source.Choices.ToList();
    }
  }
}