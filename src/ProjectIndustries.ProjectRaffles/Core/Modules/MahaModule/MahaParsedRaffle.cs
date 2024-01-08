using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.MahaModule
{
  public class MahaParsedRaffle
  {
    public MahaParsedRaffle()
    {
    }

    public MahaParsedRaffle(string product, string productId, string productImage)
    {
      Product = product;
      ProductId = productId;
      ProductImage = productImage;
    }

    [JsonProperty(nameof(Product)), BsonField(nameof(Product))]
    public string Product { get; set; }

    [JsonProperty(nameof(ProductId)), BsonField(nameof(ProductId))]
    public string ProductId { get; set; }

    [JsonProperty(nameof(ProductImage)), BsonField(nameof(ProductImage))]
    public string ProductImage { get; set; }
  }
}