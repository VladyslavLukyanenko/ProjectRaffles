using System;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  [AttributeUsage(AttributeTargets.Class)]
  public class RaffleReleaseTypeAttribute : Attribute
  {
    public RaffleReleaseTypeAttribute(RaffleReleaseType releaseType)
    {
      ReleaseType = releaseType;
    }

    public RaffleReleaseType ReleaseType { get; }
  }
}