using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public interface IClipboardService
  {
    Task SetTextAsync(string text, CancellationToken ct = default);
  }
}