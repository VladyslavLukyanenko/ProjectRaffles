using System.Collections.Generic;
using System.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.Models;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.FieldMappers
{
  public class LinearScaleGoogleFormFieldMapper : IGoogleFormFieldMapper
  {
    public bool IsFieldSupported(GoogleFormFieldType type)
    {
      return type == GoogleFormFieldType.LinearScaleField || type == GoogleFormFieldType.SelectionField;
    }

    public IEnumerable<Field> Map(GoogleFormField googleFormField)
    {
      yield return new OptionsField(googleFormField.Id, googleFormField.Name, googleFormField.Required,
        googleFormField.Options.Select(_ => _.Name));
      yield return new HiddenField(googleFormField.Id + "_sentinel", null);
    }
  }
}