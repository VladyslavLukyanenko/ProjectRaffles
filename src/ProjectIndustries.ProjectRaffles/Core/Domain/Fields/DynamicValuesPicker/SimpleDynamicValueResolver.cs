using System;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public class SimpleDynamicValueResolver : IDynamicValueResolver
  {
    public SimpleDynamicValueResolver(string name, Func<IRaffleExecutionContext, string> resolveValue)
    {
      Name = name;
      ResolveValue = context => Task.FromResult(resolveValue(context));
    }

    public string Name { get; }
    public Func<IRaffleExecutionContext, Task<string>> ResolveValue { get; }
  }
}