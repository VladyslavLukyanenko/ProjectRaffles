using System;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  [AttributeUsage(AttributeTargets.Class)]
  public class RaffleRegionAttribute : Attribute
  {
    public RaffleRegionAttribute(RaffleRegion region)
    {
      Region = region;
    }

    public RaffleRegion Region { get; }
  }
}