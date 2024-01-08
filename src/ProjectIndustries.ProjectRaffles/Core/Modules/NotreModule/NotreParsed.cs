using System.Collections.Generic;
using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NotreModule
{
    public class NotreParsed
    {
        public NotreParsed()
        {
            
        }

        public NotreParsed(List<int> raffleIds, Dictionary<int, List<string>> raffleIdSizes)
        {
            RaffleIds = raffleIds;
            RaffleIdSizes = raffleIdSizes;
        }
        
        [JsonProperty(nameof(RaffleIds)), BsonField(nameof(RaffleIds))]
        public List<int> RaffleIds { get; set; }
        
        [JsonProperty(nameof(RaffleIdSizes)), BsonField(nameof(RaffleIdSizes))]
        public Dictionary<int, List<string>> RaffleIdSizes { get; set; }
    }
}