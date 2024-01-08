using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators
{
  public abstract class ProfileDependentAccountGeneratorBase : IAccountGenerator, IRaffleExecutionContext
  {
    private Func<Profile> _profileProvider;
    public abstract IEnumerable<Field> ConfigurationFields { get; }
    public abstract IAsyncEnumerable<Account> GenerateAsync(CancellationToken ct = default);

    public async Task InitializeAsync(Func<Profile> profileProvider, CancellationToken ct = default)
    {
      _profileProvider = profileProvider;
      foreach (var f in ConfigurationFields)
      {
        await f.InitializeAsync(DependencyResolver, ct);
      }
    }

    public async Task PrepareAsync(CancellationToken ct = default)
    {
      foreach (var f in ConfigurationFields.OfType<IRequiresPreInitialization>())
      {
        await f.PrepareAsync(this, ct);
      }
    }

    public Profile Profile => _profileProvider();
    public IReadonlyDependencyResolver DependencyResolver => Locator.Current;
  }
}