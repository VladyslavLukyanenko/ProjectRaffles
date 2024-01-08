using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.ContinuumModule
{
    public class ContinuumJson
    {
        [JsonProperty(PropertyName = "First Name")]
        public string FirstName { get; set; }
        
        [JsonProperty(PropertyName = "Last Name")]
        public string LastName { get; set; }
        
        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }
        
        [JsonProperty(PropertyName = "Phone")]
        public string PhoneNumber { get; set; }
        
        [JsonProperty(PropertyName = "Street Address")]
        public string Address { get; set; }
        
        [JsonProperty(PropertyName = "City")]
        public string City { get; set; }
        
        [JsonProperty(PropertyName = "State")]
        public string State { get; set; }
        
        [JsonProperty(PropertyName = "Zip Code")]
        public string ZipCode { get; set; }
        
        [JsonProperty(PropertyName = "Shoe size")]
        public string Size { get; set; }
    }
}