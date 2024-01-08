using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
  public class DropdownTypeFormField : SingleValueTypeFormField<OptionsField>
  {
    private readonly IEnumerable<string> _options;

    public DropdownTypeFormField(string id, string label, IEnumerable<string> options)
      : base("text", TypeFormFieldDescriptor.Dropdown(id), label)
    {
      _options = options;
    }

    [JsonProperty("text")] public string Value { get; private set; }

    public override bool IsEmpty => string.IsNullOrEmpty(Value);

    public override void Prepare()
    {
      Value = FirstField.Value;
    }

    protected override IEnumerable<Field> CreateFields()
    {
      var options = _options.Select(o => new KeyValuePair<string, string>(o, o)).ToArray();
      
      yield return new OptionsField(Label, Label, false, options);
    }

    protected override void CopyValuesToClone(TypeFormField typeFormField)
    {
      ((DropdownTypeFormField) typeFormField).Value = Value;
    }
  }
}