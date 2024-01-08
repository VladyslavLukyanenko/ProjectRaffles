using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing
{
  public interface IMailChimpFieldFactory
  {
    bool IsSupports(string type);
    MailChildFieldBase Create(SettingsField field);
  }
}