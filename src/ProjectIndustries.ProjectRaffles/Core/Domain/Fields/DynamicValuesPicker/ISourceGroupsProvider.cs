using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public interface ISourceGroupsProvider
  {
    IEnumerable<IDynamicValuesGroup> Groups { get; }
  }
}