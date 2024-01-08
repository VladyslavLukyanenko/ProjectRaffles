using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Captchas
{
    public interface ICaptchaSolver
    {
        Task<CaptchaResult> SolveImageCaptchaAsync(string b64Image, CancellationToken ct);
        Task<CaptchaResult> SolveReCaptchaV2Async(string siteKey, string url, bool isVisible, CancellationToken ct);
        Task<CaptchaResult> SolveReCaptchaV3Async(string siteKey, string url, string action, double minScore, CancellationToken ct);
        Task<decimal> GetBalance(CancellationToken ct);
    }
}