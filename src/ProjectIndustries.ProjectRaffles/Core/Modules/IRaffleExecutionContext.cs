using ProjectIndustries.ProjectRaffles.Core.Domain;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public interface IRaffleExecutionContext
  {
    Profile Profile { get; }
    // Proxy Proxy { get; }
    IReadonlyDependencyResolver DependencyResolver { get; }
  }
}