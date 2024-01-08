using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields
{
  public interface IRequiresPreInitialization
  {
    Task PrepareAsync(IRaffleExecutionContext context, CancellationToken ct = default);
  }
}