using System.Collections.Generic;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
  public class PhoneNumberFormField : TextTypeFormFieldBase
  {
    public PhoneNumberFormField(string id, string label)
      : base("phone_number", TypeFormFieldDescriptor.PhoneNumber(id), label)
    {
    }

    [JsonProperty("phone_number")] public string PhoneNumber { get; private set; }

    public override void Prepare()
    {
      PhoneNumber = FirstField.Value;
    }

    public override bool IsEmpty => string.IsNullOrEmpty(PhoneNumber);

    protected override IEnumerable<Field> CreateFields()
    {
      // yield return new TextField(Descriptor.Id, Label, false); //todo: integrate
      yield return new DynamicValuesPickerField(Descriptor.Id, Label, false, groups: Pickers.All)
      {
        SelectedResolver = Pickers.ProfileShippingAddressFields.PhoneNumber
      };
    }

    protected override void CopyValuesToClone(TypeFormField typeFormField)
    {
      ((PhoneNumberFormField) typeFormField).PhoneNumber = PhoneNumber;
    }
  }
}