using System;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public interface IDynamicValueResolver
  {
    string Name { get; }
    Func<IRaffleExecutionContext, Task<string>> ResolveValue { get; }
  }
}