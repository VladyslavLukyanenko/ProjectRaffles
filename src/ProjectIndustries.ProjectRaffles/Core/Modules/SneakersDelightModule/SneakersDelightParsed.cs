using System.Collections.Generic;
using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SneakersDelightModule
{
    public class SneakersDelightParsed
    {
        public SneakersDelightParsed()
        {
            
        }
        public SneakersDelightParsed(string raffleId, Dictionary<string, string> sizeDictionary)
        {
            RaffleId = raffleId;
            SizeDictionary = sizeDictionary;
        }
        
        [JsonProperty(nameof(RaffleId)), BsonField(nameof(RaffleId))]
        public string RaffleId { get; set; }
        
        [JsonProperty(nameof(SizeDictionary)), BsonField(nameof(SizeDictionary))]
        public Dictionary<string, string> SizeDictionary { get; set; }
    }
}