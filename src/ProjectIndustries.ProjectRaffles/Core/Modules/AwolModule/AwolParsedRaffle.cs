using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AwolModule
{
  public class AwolParsedRaffle
  {
    public AwolParsedRaffle()
    {
    }

    public AwolParsedRaffle(string formId, string formBuildId)
    {
      FormId = formId;
      FormBuildId = formBuildId;
    }

    [JsonProperty(nameof(FormId)), BsonField(nameof(FormId))]
    public string FormId { get; set; }

    [JsonProperty(nameof(FormBuildId)), BsonField(nameof(FormBuildId))]
    public string FormBuildId { get; set; }
  }
}