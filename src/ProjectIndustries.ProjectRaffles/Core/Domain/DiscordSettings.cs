using System.Runtime.Serialization;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  [DataContract]
  public class DiscordSettings
  {
    [DataMember] public string WebHook { get; set; }

    public static readonly DiscordSettings Empty = new DiscordSettings();
  }
}