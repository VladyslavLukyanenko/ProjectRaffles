using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.Models;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.FieldMappers
{
  public class TimeGoogleFormFieldMapper : IGoogleFormFieldMapper
  {
    public bool IsFieldSupported(GoogleFormFieldType type)
    {
      return type == GoogleFormFieldType.TimeField;
    }

    public IEnumerable<Field> Map(GoogleFormField googleFormField)
    {
      yield return new TextField(googleFormField.Id + "_hour", googleFormField.Name + " Hour",
        googleFormField.Required);
      
      yield return new TextField(googleFormField.Id + "_minute", googleFormField.Name + " Minute",
        googleFormField.Required);
    }
  }
}