using System.Collections.Generic;
using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PattaModule
{
    public class PattaParsed
    {
        public PattaParsed()
        {
        }
        
        public PattaParsed(string productId, string productVariantId, string raffleName, string raffleId)
        {
            ProductId = productId;
            ProductVariantId = productVariantId;
            RaffleName = raffleName;
            RaffleId = raffleId;
        }
        
        [JsonProperty(nameof(ProductId)), BsonField(nameof(ProductId))]
        public string ProductId { get; set; }
        
        [JsonProperty(nameof(ProductVariantId)), BsonField(nameof(ProductVariantId))]
        public string ProductVariantId { get; set; }
        
        [JsonProperty(nameof(RaffleName)), BsonField(nameof(RaffleName))]
        public string RaffleName { get; set; }
        
        [JsonProperty(nameof(RaffleId)), BsonField(nameof(RaffleId))]
        public string RaffleId { get; set; }
    }
}