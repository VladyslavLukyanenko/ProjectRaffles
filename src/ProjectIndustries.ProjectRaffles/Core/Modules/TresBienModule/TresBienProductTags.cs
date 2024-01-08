using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TresBienModule
{
  public class TresBienProductTags
  {
    public TresBienProductTags()
    {
    }

    public TresBienProductTags(string formkey, string sku)
    {
      FormKey = formkey;
      SKU = sku;
    }

    [JsonProperty(nameof(FormKey)), BsonField(nameof(FormKey))]
    public string FormKey { get; set; }

    [JsonProperty(nameof(SKU)), BsonField(nameof(SKU))]
    public string SKU { get; set; }
  }
}