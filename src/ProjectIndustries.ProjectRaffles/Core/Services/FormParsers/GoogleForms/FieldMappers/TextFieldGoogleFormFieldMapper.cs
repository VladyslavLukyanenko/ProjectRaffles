using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.Models;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.FieldMappers
{
  public class TextFieldGoogleFormFieldMapper : IGoogleFormFieldMapper
  {
    public bool IsFieldSupported(GoogleFormFieldType type)
    {
      return type == GoogleFormFieldType.ShortAnswerField || type == GoogleFormFieldType.LongAnswerField;
    }

    public IEnumerable<Field> Map(GoogleFormField googleFormField)
    {
      yield return new DynamicValuesPickerField(googleFormField.Id, googleFormField.Name, googleFormField.Required,
        groups: Pickers.All);
    }
  }
}