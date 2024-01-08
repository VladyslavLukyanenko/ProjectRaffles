using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing
{
  public class DateFieldMailChimpFieldFactory : MailChimpFieldFactoryBase<MailChildDateFormatField>
  {
    public DateFieldMailChimpFieldFactory()
      : base("date", "birthday")
    {
    }

    protected override void SetSpecificFieldProps(SettingsField source, MailChildDateFormatField destination)
    {
      destination.DateFormat = source.DateFormat;
    }
  }
}