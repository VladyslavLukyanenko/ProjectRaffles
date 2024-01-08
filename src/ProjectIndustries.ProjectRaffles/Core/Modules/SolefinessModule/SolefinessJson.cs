using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SolefinessModule
{
    public class SolefinessJson
    {
        [JsonProperty(PropertyName = "FIRST NAME")]
        public string FirstName { get; set; }
        
        [JsonProperty(PropertyName = "LAST NAME")]
        public string LastName { get; set; }
        
        [JsonProperty(PropertyName = "EMAIL")]
        public string Email { get; set; }
        
        [JsonProperty(PropertyName = "INSTAGRAM")]
        public string Instagram { get; set; }
        
        [JsonProperty(PropertyName = "STREET ADDRESS")]
        public string Address { get; set; }
        
        [JsonProperty(PropertyName = "POST CODE")]
        public string PostCode { get; set; }
        
        [JsonProperty(PropertyName = "COUNTRY")]
        public string Country { get; set; }
        
        [JsonProperty(PropertyName = "PHONE NUMBER")]
        public string PhoneNumber { get; set; }
        
        [JsonProperty(PropertyName = "SIZE")]
        public string Size { get; set; }
        
        [JsonProperty(PropertyName = "CREDIT CARD NAME")]
        public string CreditCardName { get; set; }
        
        [JsonProperty(PropertyName = "VISA/MASTERCARD NUMBER")]
        public string CreditCardNumber { get; set; }
        
        [JsonProperty(PropertyName = "EXPIRATION (MM/YY)")]
        public string CreditCardExpiration { get; set; }
        
        [JsonProperty(PropertyName = "SECURITY CODE")]
        public string CreditCardCvv { get; set; }
        
        [JsonProperty(PropertyName = "TermsString")]
        public string TermsString { get; set; }
    }
}