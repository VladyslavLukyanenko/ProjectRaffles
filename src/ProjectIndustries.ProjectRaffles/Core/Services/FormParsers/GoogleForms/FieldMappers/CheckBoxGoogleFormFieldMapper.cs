using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.Models;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.FieldMappers
{
  public class CheckBoxGoogleFormFieldMapper : IGoogleFormFieldMapper
  {
    public bool IsFieldSupported(GoogleFormFieldType type)
    {
      return type == GoogleFormFieldType.CheckBoxField;
    }

    public IEnumerable<Field> Map(GoogleFormField googleFormField)
    {
      List<CheckboxField<string>> checkboxes = null;
      // ReSharper disable once AccessToModifiedClosure
      Func<CheckboxField, Task<bool>> validator = isChecked =>
        Task.FromResult(!googleFormField.Required || checkboxes!.Any(c => c.IsChecked));
      checkboxes = googleFormField.Options
        .Select(option =>
          new CheckboxField<string>(googleFormField.Id, option.Name + " *", option.Name, false, validator))
        .ToList();

      foreach (var checkbox in checkboxes)
      {
        yield return checkbox;
      }

      yield return new HiddenField(googleFormField.Id + "_sentinel", null);
    }
  }
}