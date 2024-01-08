using System;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Captchas
{
  public class CaptchaSolverFactory : ICaptchaSolverFactory
  {
    public ICaptchaSolver Create(CaptchaProvider provider, out CaptchaKey usedKey)
    {
      usedKey = provider.GetMostIdleKey();
      if (usedKey == null)
      {
        throw new InvalidOperationException("Can't create captcha solver because no keys available");
      }

      return Create(provider, usedKey);
    }

    public ICaptchaSolver Create(CaptchaProvider provider, CaptchaKey key)
    {
      if (provider == null)
      {
        throw new ArgumentNullException(nameof(provider));
      }

      if (key == null)
      {
        throw new ArgumentNullException(nameof(key));
      }

      return provider.ProviderName switch
      {
        SupportedCaptchaNames.AntiCaptcha => new AntiCaptchaSolver(key.Value),
        SupportedCaptchaNames.TwoCaptcha => new _2CaptchaSolver(key.Value),
        SupportedCaptchaNames.CapmonsterCloud => new CapmonsterCloudSolver(key.Value),
        _ => throw new IndexOutOfRangeException("Can't find captcha solver for name " + provider.ProviderName)
      };
    }
  }
}