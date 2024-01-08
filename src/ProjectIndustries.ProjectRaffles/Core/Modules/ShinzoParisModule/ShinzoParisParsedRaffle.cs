using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.ShinzoParisModule
{
  public class ShinzoParisParsedRaffle
  {
    public ShinzoParisParsedRaffle()
    {
    }

    public ShinzoParisParsedRaffle(string wpfc7, string wpfc7version, string locale, string unitTag, string container,
      string options, string group, string raffleId)
    {
      Wpfc7 = wpfc7;
      Wpfc7Version = wpfc7version;
      Locale = locale;
      UnitTag = unitTag;
      Container = container;
      Options = options;
      Group = group;
      RaffleId = raffleId;
    }

    [JsonProperty(nameof(Wpfc7)), BsonField(nameof(Wpfc7))]
    public string Wpfc7 { get; set; }

    [JsonProperty(nameof(Wpfc7Version)), BsonField(nameof(Wpfc7Version))]
    public string Wpfc7Version { get; set; }

    [JsonProperty(nameof(Locale)), BsonField(nameof(Locale))]
    public string Locale { get; set; }

    [JsonProperty(nameof(UnitTag)), BsonField(nameof(UnitTag))]
    public string UnitTag { get; set; }

    [JsonProperty(nameof(Container)), BsonField(nameof(Container))]
    public string Container { get; set; }

    [JsonProperty(nameof(Options)), BsonField(nameof(Options))]
    public string Options { get; set; }

    [JsonProperty(nameof(Group)), BsonField(nameof(Group))]
    public string Group { get; set; }

    [JsonProperty(nameof(RaffleId)), BsonField(nameof(RaffleId))]
    public string RaffleId { get; set; }
  }
}