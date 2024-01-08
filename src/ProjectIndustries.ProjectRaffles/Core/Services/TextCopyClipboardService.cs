using System.Threading;
using System.Threading.Tasks;
using TextCopy;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public class TextCopyClipboardService : IClipboardService
  {
    public async Task SetTextAsync(string text, CancellationToken ct = default)
    {
      await ClipboardService.SetTextAsync(text, ct);
    }
  }
}