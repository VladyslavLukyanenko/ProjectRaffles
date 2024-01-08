using System.Collections.Generic;
using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.Basket4BallersModule
{
    public class Basket4BallersParsedRaffle
    {
        public Basket4BallersParsedRaffle(){}
        
        public Basket4BallersParsedRaffle(string raffleId, string raffleToken,
            Dictionary<string, string> countryDictionary, Dictionary<string, string> stateDictionary)
        {
            RaffleId = raffleId;
            RaffleToken = raffleToken;
            CountryDictionary = countryDictionary;
            StateDictionary = stateDictionary;
        }
        
        [JsonProperty(nameof(RaffleId)), BsonField(nameof(RaffleId))]
        public string RaffleId { get; set; }
        
        [JsonProperty(nameof(RaffleToken)), BsonField(nameof(RaffleToken))]
        public string RaffleToken { get; set; }
        
        [JsonProperty(nameof(CountryDictionary)), BsonField(nameof(CountryDictionary))]
        public Dictionary<string, string> CountryDictionary { get; set; }
        
        [JsonProperty(nameof(StateDictionary)), BsonField(nameof(StateDictionary))]
        public Dictionary<string, string> StateDictionary { get; set; }
    }
}