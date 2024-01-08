using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PanthersModule
{
  public class PanthersParsedRaffle
  {
    public PanthersParsedRaffle()
    {
    }

    public PanthersParsedRaffle(string product, string image, string id, string store)
    {
      Product = product;
      Image = image;
      Id = id;
      Store = store;
    }

    [JsonProperty(nameof(Product)), BsonField(nameof(Product))]
    public string Product { get; set; }

    [JsonProperty(nameof(Image)), BsonField(nameof(Image))]
    public string Image { get; set; }

    [JsonProperty(nameof(Id)), BsonField(nameof(Id))]
    public string Id { get; set; }

    [JsonProperty(nameof(Store)), BsonField(nameof(Store))]
    public string Store { get; set; }
  }
}