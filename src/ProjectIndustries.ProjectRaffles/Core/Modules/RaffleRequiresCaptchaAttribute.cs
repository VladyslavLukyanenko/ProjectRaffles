using System;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  [AttributeUsage(AttributeTargets.Class)]
  public class RaffleRequiresCaptchaAttribute : Attribute
  {
    public RaffleRequiresCaptchaAttribute(CaptchaRequirement captchaRequirement)
    {
      CaptchaRequirement = captchaRequirement;
    }
    
    public CaptchaRequirement CaptchaRequirement { get; }
  }
}