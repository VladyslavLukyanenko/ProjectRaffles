using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Diagnostics
{
  public interface IMemoryDumpCreator
  {
    Task<bool> CreateAsync(string outputFileName, CancellationToken ct = default);
  }
}