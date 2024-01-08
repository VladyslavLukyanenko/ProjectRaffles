using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
  public abstract class TypeFormField
  {
    private IEnumerable<Field> _fields;

    protected TypeFormField(string type, TypeFormFieldDescriptor descriptor, string label)
    {
      Type = type;
      Descriptor = descriptor;
      Label = label;
    }

    public string Type { get; }
    [JsonIgnore] public abstract bool IsEmpty { get; }

    [JsonProperty("field")] public TypeFormFieldDescriptor Descriptor { get; }

    [JsonIgnore] public string Label { get; }

    [JsonIgnore] public IEnumerable<Field> Fields => _fields ??= CreateFields().ToArray();

    public abstract void Prepare();

    protected abstract IEnumerable<Field> CreateFields();

    public TypeFormField Clone()
    {
      var clone = (TypeFormField) MemberwiseClone();
      clone._fields = _fields.Select(_ => _.Clone()).ToArray();
      CopyValuesToClone(clone);

      return clone;
    }

    protected abstract void CopyValuesToClone(TypeFormField typeFormField);
  }
}