using System.Reflection;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Discord
{
  [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
  public class Author
  {
    [JsonProperty("name")] public string Name { get; set; } = "";

    [JsonProperty("url")] public string Url { get; set; } = "";

    [JsonProperty("icon_url")]
    public string IconUrl { get; set; } = "https://projectindustries.gg/images/logo-project-raffles.png";
  }
}