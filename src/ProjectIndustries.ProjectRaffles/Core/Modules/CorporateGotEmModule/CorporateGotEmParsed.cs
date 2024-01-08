using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.CorporateGotEmModule
{
    public class CorporateGotEmParsed
    {
        public CorporateGotEmParsed(){}
        
        public CorporateGotEmParsed(string formId, string formVersion, string formName, string listId)
        {
            FormId = formId;
            FormVersion = formVersion;
            FormName = formName;
            ListId = listId;
        }
        
        [JsonProperty(nameof(FormId)), BsonField(nameof(FormId))]
        public string FormId { get; set; }
        
        [JsonProperty(nameof(FormVersion)), BsonField(nameof(FormVersion))]
        public string FormVersion { get; set; }
        
        [JsonProperty(nameof(FormName)), BsonField(nameof(FormName))]
        public string FormName { get; set; }
        
        [JsonProperty(nameof(ListId)), BsonField(nameof(ListId))]
        public string ListId { get; set; }
    }
}