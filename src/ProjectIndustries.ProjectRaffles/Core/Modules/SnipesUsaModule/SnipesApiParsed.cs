using System.Collections.Generic;
using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SnipesUsaModule
{
    public class SnipesApiParsed
    {
        public SnipesApiParsed()
        {
            
        }
        
        public SnipesApiParsed(Dictionary<string, Dictionary<string, Dictionary<string, string>>> snipesApiDictionary)
        {
            SnipesApiDictionary = snipesApiDictionary;
        }
        
        [JsonProperty(nameof(SnipesApiDictionary)), BsonField(nameof(SnipesApiDictionary))]
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> SnipesApiDictionary { get; set; }
    }
}