namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketLondonInstoreModule
{
    public class DoverStreetMarketLondonInstorePayload
    {
        public DoverStreetMarketLondonInstorePayload(AddressFields addressFields,
            DoverStreetMarketLondonInstoreParsed parsed, string email, string captcha, string size, string questionAnswer)
        {
            Address = addressFields;
            Parsed = parsed;
            Email = email;
            Captcha = captcha;
            Size = size;
            QuestionAnswer = questionAnswer;
        }
        
        public AddressFields Address { get; set; }
        
        public DoverStreetMarketLondonInstoreParsed Parsed { get; set; }
        
        public string Email { get; set; }
        
        public string Captcha { get; set; }
        
        public string Size { get; set; }
       
        public string QuestionAnswer { get; set; }
    }
}