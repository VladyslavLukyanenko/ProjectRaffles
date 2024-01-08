using System;
using ProjectIndustries.ProjectRaffles.WpfUI.MapBox.MapboxNetCore;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Spatial
{
  public class AddressArea
  {
    private readonly Random _rnd = new Random((int) DateTime.Now.Ticks);
    public AddressArea(double radiusKm, GeoLocation center)
    {
      RadiusKm = radiusKm;
      Center = center;
    }

    public double RadiusKm { get; private set; }
    public GeoLocation Center { get; private set; }

    public GeoLocation GetNextRandomPointInArea()
    {
      var angle = _rnd.NextDouble() * 360;
      var distance = _rnd.NextDouble() * RadiusKm;

      var (lat, lng) = GetPointByDistanceAndHeading(Center.Latitude, Center.Longitude, angle, distance);
      
      return new GeoLocation(lat, lng);
    }


    private const double EarthRadius = 6378.1; //#Radius of the Earth km

    private Tuple<double, double> GetPointByDistanceAndHeading(double fmLat, double fmLon, double heading,
      double distanceKm)
    {
      double bearingR = heading.ToRadians();

      double latR = fmLat.ToRadians();
      double lonR = fmLon.ToRadians();

      double distanceToRadius = distanceKm / EarthRadius;

      double newLatR = Math.Asin(Math.Sin(latR) * Math.Cos(distanceToRadius)
                                 + Math.Cos(latR) * Math.Sin(distanceToRadius) * Math.Cos(bearingR));

      double newLonR = lonR + Math.Atan2(
        Math.Sin(bearingR) * Math.Sin(distanceToRadius) * Math.Cos(latR),
        Math.Cos(distanceToRadius) - Math.Sin(latR) * Math.Sin(newLatR)
      );

      return new Tuple<double, double>(newLatR.ToDegrees(), newLonR.ToDegrees());
    }
  }
}