using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Captchas
{
  public interface ICaptchaSolverFactory
  {
    ICaptchaSolver Create(CaptchaProvider provider, out CaptchaKey usedKey);
    ICaptchaSolver Create(CaptchaProvider provider, CaptchaKey key);
  }
}