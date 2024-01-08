using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.UsgStoreModule
{
    public class UsgStoreParsed
    {
        public UsgStoreParsed()
        {
            
        }
        
        public UsgStoreParsed(string firstNameId, string firstNameFieldId, string lastNameId, string lastNameFieldId,
            string sizeId, string sizeFieldId, string emailId, string emailFieldId, string phoneNumberId,
            string phoneNumberFieldId, string addressId, string addressFieldId, string formId, string formUrl)
        {
            FirstNameId = firstNameId;
            FirstNameFieldId = firstNameFieldId;
            LastNameId = lastNameId;
            LastNameFieldId = lastNameFieldId;
            SizeId = sizeId;
            SizeFieldId = sizeFieldId;
            EmailId = emailId;
            EmailFieldId = emailFieldId;
            PhoneNumberId = phoneNumberId;
            PhoneNumberFieldId = phoneNumberFieldId;
            AddressId = addressId;
            AddressFieldId = addressFieldId;

            FormId = formId;
            FormUrl = formUrl;
        }
        
        [JsonProperty(nameof(FirstNameId)), BsonField(nameof(FirstNameId))]
        public string FirstNameId { get; set; }
        
        [JsonProperty(nameof(FirstNameFieldId)), BsonField(nameof(FirstNameFieldId))]
        public string FirstNameFieldId { get; set; }
        
        [JsonProperty(nameof(LastNameId)), BsonField(nameof(LastNameId))]
        public string LastNameId { get; set; }
        
        [JsonProperty(nameof(LastNameFieldId)), BsonField(nameof(LastNameFieldId))]
        public string LastNameFieldId { get; set; }
        
        [JsonProperty(nameof(SizeId)), BsonField(nameof(SizeId))]
        public string SizeId { get; set; }
        
        [JsonProperty(nameof(SizeFieldId)), BsonField(nameof(SizeFieldId))]
        public string SizeFieldId { get; set; }
        
        [JsonProperty(nameof(EmailId)), BsonField(nameof(EmailId))]
        public string EmailId { get; set; }
        
        [JsonProperty(nameof(EmailFieldId)), BsonField(nameof(EmailFieldId))]
        public string EmailFieldId { get; set; }
        
        [JsonProperty(nameof(PhoneNumberId)), BsonField(nameof(PhoneNumberId))]
        public string PhoneNumberId { get; set; }
        
        [JsonProperty(nameof(PhoneNumberFieldId)), BsonField(nameof(PhoneNumberFieldId))]
        public string PhoneNumberFieldId { get; set; }
        
        [JsonProperty(nameof(AddressId)), BsonField(nameof(AddressId))]
        public string AddressId { get; set; }
        
        [JsonProperty(nameof(AddressFieldId)), BsonField(nameof(AddressFieldId))]
        public string AddressFieldId { get; set; }
        
        [JsonProperty(nameof(FormId)), BsonField(nameof(FormId))]
        public string FormId { get; set; }
        
        [JsonProperty(nameof(FormUrl)), BsonField(nameof(FormUrl))]
        public string FormUrl { get; set; }
    }
}