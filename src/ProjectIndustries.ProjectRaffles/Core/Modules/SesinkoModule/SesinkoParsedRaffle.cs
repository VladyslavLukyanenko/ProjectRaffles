using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SesinkoModule
{
  public class SesinkoParsedRaffle
  {
    public SesinkoParsedRaffle()
    {
    }

    public SesinkoParsedRaffle(string formId)
    {
      FormId = formId;
    }

    [JsonProperty(nameof(FormId)), BsonField(nameof(FormId))]
    public string FormId { get; set; }
  }
}