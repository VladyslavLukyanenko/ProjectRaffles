using System.Collections.Generic;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
  public class LegalFormField : SingleValueTypeFormField<CheckboxField>
  {
    public LegalFormField(string id, string label)
      : base("boolean", TypeFormFieldDescriptor.Legal(id), label)
    {
    }


    [JsonProperty("boolean")] public bool IsChecked { get; private set; }

    public override bool IsEmpty => !IsChecked;

    public override void Prepare()
    {
      IsChecked = FirstField.IsChecked;
    }

    protected override IEnumerable<Field> CreateFields()
    {
      yield return new CheckboxField(Descriptor.Id, Label, false);
    }

    protected override void CopyValuesToClone(TypeFormField typeFormField)
    {
      ((LegalFormField) typeFormField).IsChecked = IsChecked;
    }
  }
}