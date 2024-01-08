using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.Models;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.FieldMappers
{
  public class DateGoogleFormFieldMapper : IGoogleFormFieldMapper
  {
    public bool IsFieldSupported(GoogleFormFieldType type)
    {
      return type == GoogleFormFieldType.DateField;
    }

    public IEnumerable<Field> Map(GoogleFormField googleFormField)
    {
      yield return new TextField(googleFormField.Id + "_year", googleFormField.Name + " Year",
        googleFormField.Required);
      
      yield return new TextField(googleFormField.Id + "_month", googleFormField.Name + " Month",
        googleFormField.Required);
      
      yield return new TextField(googleFormField.Id + "_day", googleFormField.Name + " Day",
        googleFormField.Required);
    }
  }
}