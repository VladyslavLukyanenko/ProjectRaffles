using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Emails;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public class RandomEmailValueResolver : IDynamicValueResolver
  {
    private static readonly SemaphoreSlim Gate = new SemaphoreSlim(1, 1);
    private List<int> _indices = new List<int>();

    public RandomEmailValueResolver()
    {
      ResolveValue = context => PickRandomEmailAsync(context.DependencyResolver);
    }

    public string Name { get; } = "Random Email";
    public Func<IRaffleExecutionContext, Task<string>> ResolveValue { get; }

    private async Task<string> PickRandomEmailAsync(IReadonlyDependencyResolver serviceProvider)
    {
      var emailService = serviceProvider.GetService<IEmailRepository>();
      var emails = await emailService.Items.Connect().ToCollection().FirstOrDefaultAsync();
      var idx = await GetEmailIdxAsync(emails);
      var email = emails.ElementAt(idx);

      return email.MaterializeEmail().Value;
    }

    private async Task<int> GetEmailIdxAsync(IReadOnlyCollection<Email> emails)
    {
      try
      {
        await Gate.WaitAsync();
        if (_indices.Count == 0)
        {
          _indices.AddRange(Enumerable.Range(0, emails.Count).OrderBy(_ => Guid.NewGuid()));
        }

        var idx = _indices[^1];
        _indices.Remove(idx);
        return idx;
      }
      finally
      {
        Gate.Release();
      }
    }
  }
}