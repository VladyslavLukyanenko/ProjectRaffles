using System.Collections.Generic;
using System.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.Models;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.FieldMappers
{
  public class SelectGoogleFormFieldMapper : IGoogleFormFieldMapper
  {
    public bool IsFieldSupported(GoogleFormFieldType type)
    {
      return type == GoogleFormFieldType.ScrollMenuField;
    }

    public IEnumerable<Field> Map(GoogleFormField googleFormField)
    {
      yield return new OptionsField(googleFormField.Id, googleFormField.Name, googleFormField.Required,
        googleFormField.Options.Select(_ => _.Name));
    }
  }
}