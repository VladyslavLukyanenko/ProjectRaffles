using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketLondonModule
{
    public class DoverStreetMarketLondonSubmitPayload
    {
        public DoverStreetMarketLondonSubmitPayload(AddressFields profile, string email, DoverStreetMarketLondonParsedRaffleFields parsedraffle,
            string nonce, string captcha, string sizevalue, string variant, string questionAnswer, string shippingOption)
        {
            Profile = profile;
            Email = email;
            ParsedRaffle = parsedraffle;
            Nonce = nonce;
            SizeValue = sizevalue;
            Captcha = captcha;
            Variant = variant;
            QuestionAnswer = questionAnswer;
            ShippingOption = shippingOption;
        }

        public AddressFields Profile { get; private set; }
        public string Email { get; }
        public DoverStreetMarketLondonParsedRaffleFields ParsedRaffle { get; private set; }
        public string Nonce { get; private set; }
        public string SizeValue { get; private set; }
        public string Captcha { get; private set; }
        public string Variant { get; private set; }
        public string QuestionAnswer { get; set; }
        public string ShippingOption { get; set; }
    }
}