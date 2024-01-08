using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public interface IDynamicValuesGroup
  {
    string Group { get; }
    IReadOnlyList<IDynamicValueResolver> ValueResolvers { get; }
    public void TryCopyTo(IDynamicValuesGroup targetGroup)
    {
    }
  }

  public interface ICloneableDynamicValuesGroup
  {
    IDynamicValuesGroup CreateClone();
  }
}