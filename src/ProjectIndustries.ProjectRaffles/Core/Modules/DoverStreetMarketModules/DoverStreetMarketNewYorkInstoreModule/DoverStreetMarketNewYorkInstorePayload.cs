namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketNewYorkInstoreModule
{
    public class DoverStreetMarketNewYorkInstorePayload
    {
        public DoverStreetMarketNewYorkInstorePayload(AddressFields addressFields,
            DoverStreetMarketNewYorkInstoreParsed parsed, string email, string captcha, string size, string questionAnswer, string useMailingList)
        {
            Address = addressFields;
            Parsed = parsed;
            Email = email;
            Captcha = captcha;
            Size = size;
            QuestionAnswer = questionAnswer;
            UseMailingList = useMailingList;
        }
        
        public AddressFields Address { get; set; }
        
        public DoverStreetMarketNewYorkInstoreParsed Parsed { get; set; }
        
        public string Email { get; set; }
        
        public string Captcha { get; set; }
        
        public string Size { get; set; }
        
        public string QuestionAnswer { get; set; }
        
        public string UseMailingList { get; set; }
    }
}