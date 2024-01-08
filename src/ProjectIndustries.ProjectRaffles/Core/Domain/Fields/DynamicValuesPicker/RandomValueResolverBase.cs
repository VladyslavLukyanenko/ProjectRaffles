using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public abstract class RandomValueResolverBase : IDynamicValueResolver, IValueChangePostProcessable
  {
    private readonly SemaphoreSlim _sync = new SemaphoreSlim(1, 1);

    protected RandomValueResolverBase(string name)
    {
      Name = name;
      ResolveValue = context => PickRandomValueAsync();
    }

    public string Name { get; }
    public Func<IRaffleExecutionContext, Task<string>> ResolveValue { get; }

    public abstract Task PostProcessAsync(IReadonlyDependencyResolver dependencyResolver);
    protected abstract string GetNonUniqueRandomValue();
    protected virtual bool RequiresUniqueValues { get; } = false;

    protected virtual string GetUniqueRandomValue()
    {
      throw new NotImplementedException();
    }

    protected abstract bool CanGenerateUniqueRandomValue { get; }

    private async Task<string> PickRandomValueAsync()
    {
      if (!RequiresUniqueValues)
      {
        return GetNonUniqueRandomValue();
      }

      try
      {
        await _sync.WaitAsync();
        if (!CanGenerateUniqueRandomValue)
        {
          throw new OperationCanceledException("All unique items are used. List is empty");
        }

        return GetUniqueRandomValue();
      }
      finally
      {
        _sync.Release();
      }
    }
  }
}