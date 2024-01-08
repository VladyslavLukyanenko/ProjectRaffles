using System.Reflection;

namespace ProjectIndustries.ProjectRaffles.Core.Caches
{
  [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
  public class CacheEntry
  {
    public long ExpiresAt { get; set; }
    public string Payload { get; set; }
  }
}