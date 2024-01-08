using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
  public class MultipleChoicesTypeFormField : TypeFormField
  {
    private readonly IEnumerable<MultipleChoicesOption> _options;

    public MultipleChoicesTypeFormField(string id, string label, IEnumerable<MultipleChoicesOption> options)
      : base("choices", TypeFormFieldDescriptor.MultipleChoice(id), label)
    {
      _options = options;
    }

    [JsonProperty("choices")] public IReadOnlyList<MultipleChoicesOption> SelectedChoices { get; private set; }

    public override bool IsEmpty => !SelectedChoices?.Any() ?? true;

    public override void Prepare()
    {
      SelectedChoices = Fields.Cast<CheckboxField<MultipleChoicesOption>>()
        .Where(_ => _.IsChecked)
        .Select(_ => _.Value)
        .ToList();
    }

    protected override IEnumerable<Field> CreateFields()
    {
      return _options.Select(o => new CheckboxField<MultipleChoicesOption>(o, displayName: o.Label));
    }

    protected override void CopyValuesToClone(TypeFormField typeFormField)
    {
      ((MultipleChoicesTypeFormField) typeFormField).SelectedChoices = SelectedChoices?.ToList();
    }
  }
}