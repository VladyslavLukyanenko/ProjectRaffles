using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.HbxModule
{
  public class HbxParsedRaffle
  {
    public HbxParsedRaffle()
    {
    }

    public HbxParsedRaffle(string model, string raffleId, string productId)
    {
      Model = model;
      RaffleId = raffleId;
      ProductId = productId;
    }

    [JsonProperty(nameof(Model)), BsonField(nameof(Model))]
    public string Model { get; set; }

    [JsonProperty(nameof(RaffleId)), BsonField(nameof(RaffleId))]
    public string RaffleId { get; set; }

    [JsonProperty(nameof(ProductId)), BsonField(nameof(ProductId))]
    public string ProductId { get; set; }
  }
}