using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing.Fields
{
  public class MailChimpCheckBoxesField : MailChimpDropdownFieldBase
  {
    public override IEnumerable<Field> ConvertToFields()
    {
      IEnumerable<CheckboxField<string>> fields = null;
      // ReSharper disable AssignNullToNotNullAttribute
      // ReSharper disable once AccessToModifiedClosure
      Func<CheckboxField, Task<bool>> validator = IsRequired
        ? _ => Task.FromResult(fields.Any(f => f.IsChecked))
        : FieldValidators.AlwaysValid<CheckboxField>();

      fields = Choices.Select(c => new CheckboxField<string>(c.Value, Name, $"{Label} - {c.Label}", false, validator))
        .ToArray();
      return fields;
    }
  }
}