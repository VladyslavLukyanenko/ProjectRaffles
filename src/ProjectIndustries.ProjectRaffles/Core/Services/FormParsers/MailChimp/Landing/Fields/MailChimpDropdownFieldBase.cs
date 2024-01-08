using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing.Fields
{
  public abstract class MailChimpDropdownFieldBase : MailChildFieldBase
  {
    public IList<MailChimpDropdownOption> Choices { get; set; } = new List<MailChimpDropdownOption>();
  }
}