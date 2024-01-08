using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.Raffle4ElementosModule
{
  public class Raffle4elementosParsed
  {
    public Raffle4elementosParsed()
    {
    }

    public Raffle4elementosParsed(string form, string formid, string endpoint)
    {
      Form = form;
      FormId = formid;
      Endpoint = endpoint;
    }

    [JsonProperty(nameof(Form)), BsonField(nameof(Form))]
    public string Form { get; set; }

    [JsonProperty(nameof(FormId)), BsonField(nameof(FormId))]
    public string FormId { get; set; }
    [JsonProperty(nameof(Endpoint)), BsonField(nameof(Endpoint))]
    public string Endpoint { get; set; }
  }
}