using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AlumniModule
{
  public class AlumniParsedRaffle
  {
    public AlumniParsedRaffle()
    {
    }

    public AlumniParsedRaffle(string formId, string containerId)
    {
      FormId = formId;
      ContainerId = containerId;
    }

    [JsonProperty(nameof(FormId)), BsonField(nameof(FormId))]
    public string FormId { get; set; }

    [JsonProperty(nameof(ContainerId)), BsonField(nameof(ContainerId))]
    public string ContainerId { get; set; }
  }
}