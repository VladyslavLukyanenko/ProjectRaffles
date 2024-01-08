using System;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  [AttributeUsage(AttributeTargets.Class)]
  public class RaffleModuleVersionAttribute : Attribute
  {
    public RaffleModuleVersionAttribute(int major, int minor, int build)
    {
      Version = new Version(major, minor, build);
    }

    public Version Version { get; }
  }
}