using System.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
  public abstract class SingleValueTypeFormField<T> : TypeFormField
    where T : Field
  {
    protected SingleValueTypeFormField(string type, TypeFormFieldDescriptor descriptor, string label) : base(type,
      descriptor, label)
    {
    }

    protected T FirstField => Fields.OfType<T>().FirstOrDefault();
  }
}