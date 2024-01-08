using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WestUpNyc
{
    public class WestUpNycParsed
    {
        public WestUpNycParsed()
        {
        }

        public WestUpNycParsed(string formUrl, string sizeField)
        {
            FormUrl = formUrl;
            SizeField = sizeField;
        }
        
        [JsonProperty(nameof(FormUrl)), BsonField(nameof(FormUrl))]
        public string FormUrl { get; set; }
        
        [JsonProperty(nameof(SizeField)), BsonField(nameof(SizeField))]
        public string SizeField { get; set; }
    }
}