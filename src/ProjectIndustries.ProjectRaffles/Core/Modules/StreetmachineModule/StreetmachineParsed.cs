using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.StreetmachineModule
{
    public class StreetmachineParsed
    {
        public StreetmachineParsed()
        {
            
        }
        
        public StreetmachineParsed(string termsCheckbox, string sizeId)
        {
            TermsCheckbox = termsCheckbox;
            SizeId = sizeId;
        }
        
        [JsonProperty(nameof(TermsCheckbox)), BsonField(nameof(TermsCheckbox))]
        public string TermsCheckbox { get; set; }
        
        [JsonProperty(nameof(SizeId)), BsonField(nameof(SizeId))]
        public string SizeId { get; set; }
    }
}