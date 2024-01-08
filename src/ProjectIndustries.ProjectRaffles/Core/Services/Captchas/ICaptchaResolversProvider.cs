using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Captchas
{
  public interface ICaptchaResolversProvider
  {
    IReadOnlyList<CaptchaResolverDescriptor> SupportedCaptchaResolvers { get; }
  }
}