using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SolefinessModule
{
    public class SolefinessParsed
    {
        public SolefinessParsed()
        {
        }
        
        public SolefinessParsed(string id, string terms)
        {
            Id = id;
            Terms = terms;
        }
        [JsonProperty(nameof(Id)), BsonField(nameof(Id))]
        public string Id { get; set; }
        
        [JsonProperty(nameof(Terms)), BsonField(nameof(Terms))]
        public string Terms { get; set; }
    }
}