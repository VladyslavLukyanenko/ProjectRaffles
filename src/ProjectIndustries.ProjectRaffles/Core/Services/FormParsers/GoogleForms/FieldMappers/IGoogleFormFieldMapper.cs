using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.Models;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.FieldMappers
{
  public interface IGoogleFormFieldMapper
  {
    bool IsFieldSupported(GoogleFormFieldType type);
    IEnumerable<Field> Map(GoogleFormField googleFormField);
  }
}