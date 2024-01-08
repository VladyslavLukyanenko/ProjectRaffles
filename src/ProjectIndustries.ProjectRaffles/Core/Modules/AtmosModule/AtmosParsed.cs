using System.Collections.Generic;
using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AtmosModule
{
    public class AtmosParsed
    {
        public AtmosParsed()
        {
        }
        
        public AtmosParsed(Dictionary<string, string> models, Dictionary<string, List<string>> modelSizes,
            Dictionary<string, Dictionary<string, string>> modelStores)
        {
            Models = models;
            ModelSizes = modelSizes;
            ModelStores = modelStores;
        }
        
        [JsonProperty(nameof(Models)), BsonField(nameof(Models))]
        public Dictionary<string, string> Models { get; set; }
        
        [JsonProperty(nameof(ModelSizes)), BsonField(nameof(ModelSizes))]
        public Dictionary<string, List<string>> ModelSizes { get; set; }
        
        [JsonProperty(nameof(ModelStores)), BsonField(nameof(ModelStores))]
        public Dictionary<string, Dictionary<string, string>> ModelStores { get; set; }
    }
}