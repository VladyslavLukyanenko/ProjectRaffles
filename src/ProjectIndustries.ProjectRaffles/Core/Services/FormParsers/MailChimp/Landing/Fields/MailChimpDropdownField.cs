using System.Collections.Generic;
using System.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing.Fields
{
  public class MailChimpDropdownField : MailChimpDropdownFieldBase
  {
    public override IEnumerable<Field> ConvertToFields()
    {
      yield return new OptionsField(Name, Label, IsRequired,
        Choices.Select(c => new KeyValuePair<string, string>(c.Label, c.Value)))
      {
        PickRandom = true
      };
    }
  }
}