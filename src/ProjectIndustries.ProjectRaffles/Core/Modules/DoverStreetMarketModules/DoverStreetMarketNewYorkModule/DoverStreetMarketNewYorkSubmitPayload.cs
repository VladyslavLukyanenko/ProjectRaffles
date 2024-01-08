using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketNewYorkModule
{
    public class DoverStreetMarketNewYorkSubmitPayload
    {
        public DoverStreetMarketNewYorkSubmitPayload(AddressFields profile, string email, DoverStreetMarketNewYorkParsedRaffleFields parsedraffle,
            string nonce, string captcha, string sizevalue, string variant, string questionAnswer, string mailingList)
        {
            Profile = profile;
            Email = email;
            ParsedRaffle = parsedraffle;
            Nonce = nonce;
            SizeValue = sizevalue;
            Captcha = captcha;
            Variant = variant;
            QuestionAnswer = questionAnswer;
            UseMailingList = mailingList;
        }

        public AddressFields Profile { get; private set; }
        public string Email { get; }
        public DoverStreetMarketNewYorkParsedRaffleFields ParsedRaffle { get; private set; }
        public string Nonce { get; private set; }
        public string SizeValue { get; private set; }
        public string Captcha { get; private set; }
        public string Variant { get; private set; }
        public string QuestionAnswer { get; }
        public string UseMailingList { get; }
    }
}