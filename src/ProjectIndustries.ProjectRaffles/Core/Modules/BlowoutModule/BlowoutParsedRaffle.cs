using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.BlowoutModule
{
  public class BlowoutParsedRaffle
  {
    public BlowoutParsedRaffle()
    {
    }

    public BlowoutParsedRaffle(string postUrl, string contestId)
    {
      PostUrl = postUrl;
      ContestId = contestId;
    }

    [JsonProperty(nameof(PostUrl)), BsonField(nameof(PostUrl))]
    public string PostUrl { get; set; }

    [JsonProperty(nameof(ContestId)), BsonField(nameof(ContestId))]
    public string ContestId { get; set; }
  }
}