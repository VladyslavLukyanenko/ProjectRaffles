using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TravisScottModule
{
    public class TravisScottParsed
    {
        public TravisScottParsed(){}
        public TravisScottParsed(string formEndpoint, string productId, string kind)
        {
            FormEndpoint = formEndpoint;
            ProductId = productId;
            Kind = kind;
        }
        [JsonProperty(nameof(FormEndpoint)), BsonField(nameof(FormEndpoint))]
        public string FormEndpoint { get; set; }
        
        [JsonProperty(nameof(ProductId)), BsonField(nameof(ProductId))]
        public string ProductId { get; set; }
        
        [JsonProperty(nameof(Kind)), BsonField(nameof(Kind))]
        public string Kind { get; set; }
    }
}