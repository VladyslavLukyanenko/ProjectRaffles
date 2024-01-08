using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.StarWarsCelebrationModule
{
    public class StarWarsCelebrationParsed
    {
        public StarWarsCelebrationParsed()
        {
        }

        public StarWarsCelebrationParsed(string eventId, string formId, string clientId)
        {
            EventId = eventId;
            FormId = formId;
            ClientId = clientId;
        }
        
        [JsonProperty(nameof(EventId)), BsonField(nameof(EventId))]
        public string EventId { get; set; }
        
        [JsonProperty(nameof(FormId)), BsonField(nameof(FormId))]
        public string FormId { get; set; }
        
        [JsonProperty(nameof(ClientId)), BsonField(nameof(ClientId))]
        public string ClientId { get; set; }
    }
}