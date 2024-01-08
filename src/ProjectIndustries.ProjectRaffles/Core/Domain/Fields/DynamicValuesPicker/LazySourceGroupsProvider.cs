using System;
using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public class LazySourceGroupsProvider : ISourceGroupsProvider
  {
    private readonly Lazy<IEnumerable<DynamicValuesGroup>> _groupsProvider;

    public LazySourceGroupsProvider(Lazy<IEnumerable<DynamicValuesGroup>> groupsProvider)
    {
      _groupsProvider = groupsProvider;
    }

    public IEnumerable<IDynamicValuesGroup> Groups => _groupsProvider.Value;
  }
}