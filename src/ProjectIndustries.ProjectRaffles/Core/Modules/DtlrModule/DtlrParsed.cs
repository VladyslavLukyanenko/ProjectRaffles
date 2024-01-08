using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DtlrModule
{
    public class DtlrParsed
    {
        public DtlrParsed()
        {
        }

        public DtlrParsed(string formId, string firstnameInput, string lastNameInput, string emailInput,
            string verifyEmailInput, string birthYearInput, string phoneNumberInput, string addressLine1Input,
            string cityInput, string stateInput, string postCodeInput, string countryInput, string genderInput,
            string sizeInput, string valueLessInput19, string valueLessInput20, string gformUnique, string formState,
            string targetPage, string sourcePage, string fieldValues)
        {
            FormId = formId;
            FirstNameInput = firstnameInput;
            LastNameInput = lastNameInput;
            EmailInput = emailInput;
            VerifyEmailInput = verifyEmailInput;
            BirthYearInput = birthYearInput;
            PhoneNumberInput = phoneNumberInput;
            AddressLine1Input = addressLine1Input;
            CityInput = cityInput;
            StateInput = stateInput;
            PostCodeInput = postCodeInput;
            CountryInput = countryInput;
            GenderInput = genderInput;
            SizeInput = sizeInput;
            ValueLessInput19 = valueLessInput19;
            ValueLessInput20 = valueLessInput20;
            gFormUnique = gformUnique;
            FormState = formState;
            TargetPage = targetPage;
            SourcePage = sourcePage;
            FieldValues = fieldValues;
        }
        [JsonProperty(nameof(FormId)), BsonField(nameof(FormId))]
        public string FormId { get; set; }
        
        [JsonProperty(nameof(FirstNameInput)), BsonField(nameof(FirstNameInput))]
        public string FirstNameInput { get; set; }
        
        [JsonProperty(nameof(LastNameInput)), BsonField(nameof(LastNameInput))]
        public string LastNameInput { get; set; }
        
        [JsonProperty(nameof(EmailInput)), BsonField(nameof(EmailInput))]
        public string EmailInput { get; set; }
        
        [JsonProperty(nameof(VerifyEmailInput)), BsonField(nameof(VerifyEmailInput))]
        public string VerifyEmailInput { get; set; }
        
        [JsonProperty(nameof(BirthYearInput)), BsonField(nameof(BirthYearInput))]
        public string BirthYearInput { get; set; }
        
        [JsonProperty(nameof(PhoneNumberInput)), BsonField(nameof(PhoneNumberInput))]
        public string PhoneNumberInput { get; set; }
        
        [JsonProperty(nameof(AddressLine1Input)), BsonField(nameof(AddressLine1Input))]
        public string AddressLine1Input { get; set; }
        
        [JsonProperty(nameof(CityInput)), BsonField(nameof(CityInput))]
        public string CityInput { get; set; }
        
        [JsonProperty(nameof(StateInput)), BsonField(nameof(StateInput))]
        public string StateInput { get; set; }
        
        [JsonProperty(nameof(PostCodeInput)), BsonField(nameof(PostCodeInput))]
        public string PostCodeInput { get; set; }
        
        [JsonProperty(nameof(CountryInput)), BsonField(nameof(CountryInput))]
        public string CountryInput { get; set; }
        
        [JsonProperty(nameof(GenderInput)), BsonField(nameof(GenderInput))]
        public string GenderInput { get; set; }
        
        [JsonProperty(nameof(SizeInput)), BsonField(nameof(SizeInput))]
        public string SizeInput { get; set; }
        
        [JsonProperty(nameof(ValueLessInput19)), BsonField(nameof(ValueLessInput19))]
        public string ValueLessInput19 { get; set; }
        
        [JsonProperty(nameof(ValueLessInput20)), BsonField(nameof(ValueLessInput20))]
        public string ValueLessInput20 { get; set; }
        
        [JsonProperty(nameof(gFormUnique)), BsonField(nameof(gFormUnique))]
        public string gFormUnique { get; set; }
        
        [JsonProperty(nameof(FormState)), BsonField(nameof(FormState))]
        public string FormState { get; set; }
        
        [JsonProperty(nameof(TargetPage)), BsonField(nameof(TargetPage))]
        public string TargetPage { get; set; }
        
        [JsonProperty(nameof(SourcePage)), BsonField(nameof(SourcePage))]
        public string SourcePage { get; set; }
        
        [JsonProperty(nameof(FieldValues)), BsonField(nameof(FieldValues))]
        public string FieldValues { get; set; }
        
    }
}