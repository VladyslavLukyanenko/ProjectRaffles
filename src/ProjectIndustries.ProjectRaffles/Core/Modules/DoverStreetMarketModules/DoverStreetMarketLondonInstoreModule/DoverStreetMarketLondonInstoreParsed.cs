using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketLondonInstoreModule
{
    public class DoverStreetMarketLondonInstoreParsed
    {
        public DoverStreetMarketLondonInstoreParsed()
        {
        }
        
        public DoverStreetMarketLondonInstoreParsed(string formkey, string viewkey, string viewparam, string fullNameField, string emailField, string phoneNumberField,
            string sizeField, string zipcodeField, string mailinglist, string questionField)
        {
            Form = formkey;
            Viewkey = viewkey;
            Viewparam = viewparam;
            FullnameField = fullNameField;
            PhoneNumberField = phoneNumberField;
            EmailField = emailField;
            SizeField = sizeField;
            ZipCodeField = zipcodeField;
            MailingList = mailinglist;
            QuestionField = questionField;
        }
        [JsonProperty(nameof(Form)), BsonField(nameof(Form))]
        public string Form { get; set; }

        [JsonProperty(nameof(Viewkey)), BsonField(nameof(Viewkey))]
        public string Viewkey { get; set; }

        [JsonProperty(nameof(Viewparam)), BsonField(nameof(Viewparam))]
        public string Viewparam { get; set; }

        [JsonProperty(nameof(FullnameField)), BsonField(nameof(FullnameField))]
        public string FullnameField { get; set; }

        [JsonProperty(nameof(PhoneNumberField)), BsonField(nameof(PhoneNumberField))]
        public string PhoneNumberField { get; set; }
    
        [JsonProperty(nameof(EmailField)), BsonField(nameof(EmailField))]
        public string EmailField { get; set; }

        [JsonProperty(nameof(AddressField)), BsonField(nameof(AddressField))]
        public string AddressField { get; set; }

        [JsonProperty(nameof(SizeField)), BsonField(nameof(SizeField))]
        public string SizeField { get; set; }

        [JsonProperty(nameof(ZipCodeField)), BsonField(nameof(ZipCodeField))]
        public string ZipCodeField { get; set; }

        [JsonProperty(nameof(MailingList)), BsonField(nameof(MailingList))]
        public string MailingList { get; set; }
        
        [JsonProperty(nameof(QuestionField)), BsonField(nameof(QuestionField))]
        public string QuestionField { get; set; }
    }
}