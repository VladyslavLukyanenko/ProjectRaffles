using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public interface IPreloadService
  {
    Task PreAuthPreloadAsync(CancellationToken ct = default);
    Task PostAuthPreloadAsync(CancellationToken ct = default);
  }
}