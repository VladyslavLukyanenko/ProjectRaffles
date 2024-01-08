using System.Collections.Generic;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
  public class EmailFormField : SingleValueTypeFormField<DynamicValuesPickerField>
  {
    public EmailFormField(string id, string label)
      : base("email", TypeFormFieldDescriptor.Email(id), label)
    {
    }

    [JsonProperty("email")] public string Email { get; private set; }

    public override void Prepare()
    {
      Email = FirstField.Value;
    }

    public override bool IsEmpty => string.IsNullOrEmpty(Email);

    protected override IEnumerable<Field> CreateFields()
    {
      yield return new DynamicValuesPickerField(Descriptor.Id, Label, false, groups: Pickers.All)
      {
        SelectedResolver = Pickers.Misc.Email
      };
    }

    protected override void CopyValuesToClone(TypeFormField typeFormField)
    {
      ((EmailFormField) typeFormField).Email = Email;
    }
  }
}