using System;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public static class StringExtensions
  {
    public static string UriDataEscape(this string s)
    {
      return Uri.EscapeDataString(s);
    }

    public static string UriEscape(this string s)
    {
      return Uri.EscapeUriString(s);
    }

    public static string Slugify(this string s)
    {
      return UriSafeString.Slugify(0, int.MaxValue, s);
    }
  }
}