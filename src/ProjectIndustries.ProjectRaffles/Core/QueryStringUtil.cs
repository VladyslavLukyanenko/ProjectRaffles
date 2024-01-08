using System;
using System.Linq;

namespace ProjectIndustries.ProjectRaffles.Core
{
  public static class QueryStringUtil
  {
    public static ILookup<string, string> Parse(string query)
    {
      if (query.StartsWith("?"))
      {
        query = query.Substring(1);
      }

      return query.Split('&', StringSplitOptions.RemoveEmptyEntries)
        .Select(pair => pair.Split('=', StringSplitOptions.RemoveEmptyEntries))
        .ToLookup(t => Uri.UnescapeDataString(t[0]), t => t.Length == 2 ? Uri.UnescapeDataString(t[1]) : null);
    }
  }
}