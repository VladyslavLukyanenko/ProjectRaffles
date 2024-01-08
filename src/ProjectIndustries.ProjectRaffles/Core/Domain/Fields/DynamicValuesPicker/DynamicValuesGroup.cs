using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public class DynamicValuesGroup : IDynamicValuesGroup
  {
    public DynamicValuesGroup(string @group, IReadOnlyList<IDynamicValueResolver> valueResolvers)
    {
      Group = group;
      ValueResolvers = valueResolvers;
    }

    public string Group { get; }
    public IReadOnlyList<IDynamicValueResolver> ValueResolvers { get; }
  }
}