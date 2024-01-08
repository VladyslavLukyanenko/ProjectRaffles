using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SuccezzModule
{
    public class SuccezzJson
    {
        [JsonProperty(PropertyName = "First Name")]
        public string FirstName { get; set; }
        
        [JsonProperty(PropertyName = "Last Name")]
        public string LastName { get; set; }
        
        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }
        
        [JsonProperty(PropertyName = "Phone Number")]
        public string PhoneNumber { get; set; }
        
        [JsonProperty(PropertyName = "Size")]
        public string Size { get; set; }
        
        [JsonProperty(PropertyName = "Address")]
        public Dictionary<string, string>[] Address { get; set; }
    }
/*
    public class SuccezzJsonAddress
    {
        [JsonProperty(PropertyName = "Address line 1")]
        public string AddressLine1 { get; set; }
        
        [JsonProperty(PropertyName = "Address line 2")]
        public string AddressLine2 { get; set; }
        
        [JsonProperty(PropertyName = "City")]
        public string City { get; set; }
        
        [JsonProperty(PropertyName = "Province")]
        public string Province { get; set; }
        
        [JsonProperty(PropertyName = "Zip code")]
        public string ZipCode { get; set; }
        
        [JsonProperty(PropertyName = "Country")]
        public string Country { get; set; }
    } */
}