using System;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields
{
  public class CustomListPickerField : Field<CustomList>
  {
    public CustomListPickerField()
    {
    }

    public CustomListPickerField(string systemName, string displayName, bool isRequired,
      Func<CustomList, Task<bool>> validator = null)
      : base(systemName, displayName, isRequired, validator)
    {
    }

    public override string ValueId => Value?.Id.ToString();
    public override string DisplayValue => ValueId;
  }
}