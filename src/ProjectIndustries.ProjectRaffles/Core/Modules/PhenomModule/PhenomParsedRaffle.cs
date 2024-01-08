using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PhenomModule
{
  public class PhenomParsedRaffle
  {
    public PhenomParsedRaffle()
    {
    }

    public PhenomParsedRaffle(string formBuildId, string formId)
    {
      FormBuildId = formBuildId;
      FormId = formId;
    }

    [JsonProperty(nameof(FormBuildId)), BsonField(nameof(FormBuildId))]
    public string FormBuildId { get; set; }

    [JsonProperty(nameof(FormId)), BsonField(nameof(FormId))]
    public string FormId { get; set; }
  }
}