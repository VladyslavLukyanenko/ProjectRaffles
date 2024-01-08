using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.FenomModule
{
    public class FenomSizeIdentifiers
    {
        public FenomSizeIdentifiers(string productid, string groupid, string attributionid)
        {
            ProductId = productid;
            GroupId = groupid;
            AttributionId = attributionid;
        }
        
        [JsonProperty(nameof(ProductId)), BsonField(nameof(ProductId))]
        public string ProductId { get; }
        
        [JsonProperty(nameof(GroupId)), BsonField(nameof(GroupId))]
        public string GroupId { get; }
        
        [JsonProperty(nameof(AttributionId)), BsonField(nameof(AttributionId))]
        public string AttributionId { get; }
    }
}