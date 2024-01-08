using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WoeiModule
{
    public class WoeiParsed
    {
        public WoeiParsed()
        {
        }

        public WoeiParsed(string product, string productimage, string productid)
        {
            Product = product;
            ProductImage = productimage;
            ProductId = productid;
        }
        
        [JsonProperty(nameof(Product)), BsonField(nameof(Product))]
        public string Product { get; set; }
        
        [JsonProperty(nameof(ProductImage)), BsonField(nameof(ProductImage))]
        public string ProductImage { get; set; }
        
        [JsonProperty(nameof(ProductId)), BsonField(nameof(ProductId))]
        public string ProductId { get; set; }
    }
}