using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.Models;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.FieldMappers
{
  public class SkipGoogleFormFieldMapper : IGoogleFormFieldMapper
  {
    public bool IsFieldSupported(GoogleFormFieldType type)
    {
      return type == GoogleFormFieldType.TitleField || type == GoogleFormFieldType.GridField
                                                    || type == GoogleFormFieldType.PictureField
                                                    || type == GoogleFormFieldType.SectionField
                                                    || type == GoogleFormFieldType.UploadField
                                                    || type == GoogleFormFieldType.VideoField;
    }

    public IEnumerable<Field> Map(GoogleFormField googleFormField)
    {
      yield return new HiddenField(googleFormField.Id + "_sentinel", null);
    }
  }
}