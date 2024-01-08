using System.Collections.Generic;
using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WelcomeSkateModule
{
    public class WelcomeSkateParsed
    {
        public WelcomeSkateParsed()
        {}
        
        public WelcomeSkateParsed(Dictionary<string, string> idxDictionary, string raffleId, string appId)
        {
            IdxDictionary = idxDictionary;
            RaffleId = raffleId;
            AppId = appId;
        }
        [JsonProperty(nameof(IdxDictionary)), BsonField(nameof(IdxDictionary))]
        public Dictionary<string,string> IdxDictionary { get; set; }
        
        [JsonProperty(nameof(RaffleId)), BsonField(nameof(RaffleId))]
        public string RaffleId { get; set; }
        
        [JsonProperty(nameof(AppId)), BsonField(nameof(AppId))]
        public string AppId { get; set; }
        
    }
}