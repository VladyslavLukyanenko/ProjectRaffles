using System;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Spatial
{
  public static class GeoExtensions
  {
    public static double ToRadians(this double degrees)
    {
      return (Math.PI / 180) * degrees;
    }

    public static double ToDegrees(this double radians)
    {
      return (180 / Math.PI) * radians;
    }
  }
}