using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TypeFormsModule
{
  public class TypeFormsSubmitPayload
  {
    private readonly TypeFormsSubmission _submission;

    public TypeFormsSubmitPayload(TypeFormsSubmission submission, IEnumerable<TypeFormField> fields)
    {
      _submission = submission;
      var typeFormFields = fields as TypeFormField[] ?? fields.ToArray();
      foreach (var field in typeFormFields)
      {
        field.Prepare();
      }

      Fields = typeFormFields.Where(f => !f.IsEmpty);
    }

    public string Signature => _submission.Signature;
    [JsonProperty("landed_at")] public int LandedAt => _submission.LandedAt;
    [JsonProperty("form_id")] public string FormId => _submission.FormId;
    [JsonProperty("answers")] public IEnumerable<TypeFormField> Fields { get; }
  }
}