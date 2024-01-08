using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Captchas
{
  public interface ICaptchaSolveService
  {
    Task<string> SolveImageCaptchaAsync(string b64Image, CancellationToken ct);
    Task<string> SolveReCaptchaV2Async(string siteKey, string url, bool isInvisible, CancellationToken ct);
    Task<string> SolveReCaptchaV3Async(string siteKey, string url, string action, double minScore, CancellationToken ct);
  }
}