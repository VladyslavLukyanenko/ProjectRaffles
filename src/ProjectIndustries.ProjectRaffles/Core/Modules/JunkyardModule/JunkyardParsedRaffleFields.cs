using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.JunkyardModule
{
  public class JunkyardParsedRaffleFields
  {
    public JunkyardParsedRaffleFields()
    {
    }

    public JunkyardParsedRaffleFields(string formkey, string raffleid, string sizevalue, string botfield,
      string botvalue)
    {
      FormKey = formkey;
      RaffleId = raffleid;
      SizeValue = sizevalue;
      BotField = botfield;
      BotValue = botvalue;
    }

    [JsonProperty(nameof(FormKey)), BsonField(nameof(FormKey))]
    public string FormKey { get; set; }

    [JsonProperty(nameof(RaffleId)), BsonField(nameof(RaffleId))]
    public string RaffleId { get; set; }

    [JsonProperty(nameof(SizeValue)), BsonField(nameof(SizeValue))]
    public string SizeValue { get; set; }

    [JsonProperty(nameof(BotField)), BsonField(nameof(BotField))]
    public string BotField { get; set; }

    [JsonProperty(nameof(BotValue)), BsonField(nameof(BotValue))]
    public string BotValue { get; set; }
  }
}