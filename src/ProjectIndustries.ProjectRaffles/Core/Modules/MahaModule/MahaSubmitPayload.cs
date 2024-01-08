using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.MahaModule
{
    public class MahaSubmitPayload
    {
        public MahaSubmitPayload(AddressFields profile, Account account, MahaParsedRaffle parsedRaffle, string size, string instagram,
            string captcha, string shipping)
        {
            Profile = profile;
            Account = account;
            ParsedRaffle = parsedRaffle;
            Size = size;
            Instagram = instagram;
            Captcha = captcha;
            Shipping = shipping;
        }
        
        public AddressFields Profile { get; }
        public Account Account { get; }
        public MahaParsedRaffle ParsedRaffle { get; }
        public string Size { get; }
        public string Instagram { get; }
        public string Captcha { get; }
        public string Shipping { get; }
    }
}