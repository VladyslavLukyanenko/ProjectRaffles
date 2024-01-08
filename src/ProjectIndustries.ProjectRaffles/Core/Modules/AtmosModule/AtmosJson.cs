using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AtmosModule
{
    public class AtmosJson
    {
        [JsonProperty(PropertyName = "release")]
        public string release { get; set; }
        
        [JsonProperty(PropertyName = "size")]
        public string size { get; set; }
        
        [JsonProperty(PropertyName = "email")]
        public string email { get; set; }
        
        [JsonProperty(PropertyName = "confirmEmail")]
        public string confirmEmail { get; set; }
        
        [JsonProperty(PropertyName = "firstName")]
        public string firstName { get; set; }
        
        [JsonProperty(PropertyName = "lastName")]
        public string lastName { get; set; }
        
        [JsonProperty(PropertyName = "zipCode")]
        public string zipCode { get; set; }
        
        [JsonProperty(PropertyName = "instagramHandle")]
        public string instagramHandle { get; set; }
        
        [JsonProperty(PropertyName = "phoneNumber")]
        public string phoneNumber { get; set; }
        
        [JsonProperty(PropertyName = "recaptcha")]
        public string recaptcha { get; set; }
        
        [JsonProperty(PropertyName = "store")]
        public string store { get; set; }
        
        [JsonProperty(PropertyName = "address1")]
        public string address1 { get; set; }
        
        [JsonProperty(PropertyName = "address2")]
        public string address2 { get; set; }
        
        [JsonProperty(PropertyName = "state")]
        public string state { get; set; }
        
        [JsonProperty(PropertyName = "city")]
        public string city { get; set; }
    }
}