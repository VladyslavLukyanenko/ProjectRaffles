using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
  public class OpinionScaleFormField : SingleValueTypeFormField<OptionsField>
  {
    private readonly int _maxValue;
    private readonly bool _startsWithOne;

    public OpinionScaleFormField(string id, string label, int maxValue, bool startsWithOne)
      : base("number", TypeFormFieldDescriptor.OpinionScale(id), label)
    {
      _maxValue = maxValue;
      _startsWithOne = startsWithOne;
    }

    [JsonProperty("number")] public int? Value { get; private set; }

    public override bool IsEmpty => !Value.HasValue;

    public override void Prepare()
    {
      var raw = FirstField.Value;
      Value = int.Parse(raw);
    }

    protected override IEnumerable<Field> CreateFields()
    {
      var options = Enumerable.Range(_startsWithOne ? 1 : 0, _startsWithOne ? _maxValue : _maxValue + 1)
        .Select(n => n.ToString());

      yield return new OptionsField(Descriptor.Id, Label, true, options);
    }

    protected override void CopyValuesToClone(TypeFormField typeFormField)
    {
      var field = ((OpinionScaleFormField) typeFormField);
      field.Value = Value;
    }
  }
}