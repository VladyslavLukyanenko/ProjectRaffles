using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.OffTheHookModule
{
  public class OffTheHookParsedRaffle
  {
    public OffTheHookParsedRaffle()
    {
    }

    public OffTheHookParsedRaffle(string raffleId, string source, string formversion)
    {
      RaffleId = raffleId;
      Source = source;
      FormVersion = formversion;
    }

    [JsonProperty(nameof(RaffleId)), BsonField(nameof(RaffleId))]
    public string RaffleId { get; set; }

    [JsonProperty(nameof(Source)), BsonField(nameof(Source))]
    public string Source { get; set; }

    [JsonProperty(nameof(FormVersion)), BsonField(nameof(FormVersion))]
    public string FormVersion { get; set; }
  }
}