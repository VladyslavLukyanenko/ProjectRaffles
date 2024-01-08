using System;
using System.Reflection;

namespace ProjectIndustries.ProjectRaffles.Core.Caches
{
  [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
  public class LocalCacheEntry
  {
    public string Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public string Payload { get; set; }
    public bool IsExpired() => ExpiresAt < DateTimeOffset.UtcNow;
  }
}