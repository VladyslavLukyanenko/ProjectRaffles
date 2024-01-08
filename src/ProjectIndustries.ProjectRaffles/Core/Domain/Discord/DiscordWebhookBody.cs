using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Discord
{
  [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
  public class DiscordWebhookBody
  {
    [JsonProperty("content")] public string Content { get; set; } = "";

    [JsonProperty("username")] public string Username { get; set; } = "Project Raffles";

    [JsonProperty("avatar_url")]
    public string AvatarUrl { get; set; } = "https://projectindustries.gg/images/logo-project-raffles.png";

    [JsonProperty("embeds")] public List<Embed> Embeds { get; set; } = new List<Embed>();
  }
}