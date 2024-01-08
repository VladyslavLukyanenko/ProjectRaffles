using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SnkrKuwaitModule
{
    public class SnkrKuwaitParsed
    {
        public SnkrKuwaitParsed(string formId, string nameField, string emailField, string accountPasswordField,
            string genderField, string ageGroupField, string dobField, string mobileField, string idField,
            string addressLine1Field, string addressLine2Field, string cityField, string stateField, string postField,
            string countryField, string sizeField)
        {
            FormId = formId;
            NameField = nameField;
            EmailField = emailField;
            AccountPasswordField = accountPasswordField;
            GenderField = genderField;
            AgeGroupField = ageGroupField;
            DateOfBirthField = dobField;
            MobileField = mobileField;
            IdField = idField;
            AddressLine1Field = addressLine1Field;
            AddressLine2Field = addressLine2Field;
            CityField = cityField;
            StateField = stateField;
            PostField = postField;
            CountryField = countryField;
            SizeField = sizeField;
        }
        [JsonProperty(nameof(FormId)), BsonField(nameof(FormId))]
        public string FormId { get; set; }
        
        [JsonProperty(nameof(NameField)), BsonField(nameof(NameField))]
        public string NameField { get; set; }
        
        [JsonProperty(nameof(EmailField)), BsonField(nameof(EmailField))]
        public string EmailField { get; set; }
        
        [JsonProperty(nameof(AccountPasswordField)), BsonField(nameof(AccountPasswordField))]
        public string AccountPasswordField { get; set; }
        
        [JsonProperty(nameof(GenderField)), BsonField(nameof(GenderField))]
        public string GenderField { get; set; }
        
        [JsonProperty(nameof(AgeGroupField)), BsonField(nameof(AgeGroupField))]
        public string AgeGroupField { get; set; }
        
        [JsonProperty(nameof(DateOfBirthField)), BsonField(nameof(DateOfBirthField))]
        public string DateOfBirthField { get; set; }
        
        [JsonProperty(nameof(MobileField)), BsonField(nameof(MobileField))]
        public string MobileField { get; set; }
        
        [JsonProperty(nameof(IdField)), BsonField(nameof(IdField))]
        public string IdField { get; set; }
        
        [JsonProperty(nameof(AddressLine1Field)), BsonField(nameof(AddressLine1Field))]
        public string AddressLine1Field { get; set; }
        
        [JsonProperty(nameof(AddressLine2Field)), BsonField(nameof(AddressLine2Field))]
        public string AddressLine2Field { get; set; }
        
        [JsonProperty(nameof(CityField)), BsonField(nameof(CityField))]
        public string CityField { get; set; }
        
        [JsonProperty(nameof(StateField)), BsonField(nameof(StateField))]
        public string StateField { get; set; }
        
        [JsonProperty(nameof(PostField)), BsonField(nameof(PostField))]
        public string PostField { get; set; }
        
        [JsonProperty(nameof(CountryField)), BsonField(nameof(CountryField))]
        public string CountryField { get; set; }
        
        [JsonProperty(nameof(SizeField)), BsonField(nameof(SizeField))]
        public string SizeField { get; set; }

    }
}