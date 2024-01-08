using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.CopdateModule
{
    public class CopdateParsedRaffle
    {
        public CopdateParsedRaffle()
        {
        }

        public CopdateParsedRaffle(string gid, string entry, string entryid, string product, string store)
        {
            GId = gid;
            Entry = entry;
            EntryId = entryid;
            Product = product;
            Store = store;
        }
        
        [JsonProperty(nameof(GId)), BsonField(nameof(GId))]
        public string GId { get; set; }
        
        [JsonProperty(nameof(Entry)), BsonField(nameof(Entry))]
        public string Entry { get; set; }
        
        [JsonProperty(nameof(EntryId)), BsonField(nameof(EntryId))]
        public string EntryId { get; set; }
        
        [JsonProperty(nameof(Product)), BsonField(nameof(Product))]
        public string Product { get; set; }
        
        [JsonProperty(nameof(Store)), BsonField(nameof(Store))]
        public string Store { get; set; }
    }
}