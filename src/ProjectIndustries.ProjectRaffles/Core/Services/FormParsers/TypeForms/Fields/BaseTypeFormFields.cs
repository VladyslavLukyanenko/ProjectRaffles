using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
  public class TextTypeFormFieldBase : SingleValueTypeFormField<DynamicValuesPickerField>
  {
    protected TextTypeFormFieldBase(string type, TypeFormFieldDescriptor typeFormFieldDescriptor, string label)
      : base(type, typeFormFieldDescriptor, label)
    {
    }

    public string Text { get; private set; }

    public override void Prepare()
    {
      Text = FirstField.Value;
    }

    public override bool IsEmpty => string.IsNullOrEmpty(Text);

    protected override IEnumerable<Field> CreateFields()
    {
      // yield return new TextField(Descriptor.Id, Label, false);
      yield return new DynamicValuesPickerField(Descriptor.Id, Label, false, groups: Pickers.All); //todo: integrate
    }

    protected override void CopyValuesToClone(TypeFormField typeFormField)
    {
      ((TextTypeFormFieldBase) typeFormField).Text = Text;
    }
  }
}