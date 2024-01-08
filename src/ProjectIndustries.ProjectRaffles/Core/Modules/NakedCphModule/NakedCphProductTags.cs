using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NakedCphModule
{
  public class NakedCphProductTags
  {
    public NakedCphProductTags()
    {
    }

    public NakedCphProductTags(string tags, string token)
    {
      Tags = tags;
      Token = token;
    }

    [JsonProperty(nameof(Tags)), BsonField(nameof(Tags))]
    public string Tags { get; set; }

    [JsonProperty(nameof(Token)), BsonField(nameof(Token))]
    public string Token { get; set; }
  }
}