using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.IncuModule
{
    public class IncuParsedRaffle
    {
        public IncuParsedRaffle()
        {}
        
        public IncuParsedRaffle(string submitUrl, string sheet)
        {
            SubmitUrl = submitUrl;
            Sheet = sheet;
        }
        [JsonProperty(nameof(SubmitUrl)), BsonField(nameof(SubmitUrl))]
        public string SubmitUrl { get; set; }
        
        [JsonProperty(nameof(Sheet)), BsonField(nameof(Sheet))]
        public string Sheet { get; set; }
    }
}