using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AmigoSkateshopModule
{
    public class AmigoSkateshopSubmitPayload
    {
        public AmigoSkateshopSubmitPayload(AddressFields profile, string email, AmigoSkateShopParsedForm parsedForm, string size, string captcha, string instagram, string shipping, string raffleUrl)
        {
            Profile = profile;
            ParsedForm = parsedForm;
            Email = email;
            Size = size;
            Captcha = captcha;
            Instagram = instagram;
            Shipping = shipping;
            RaffleUrl = raffleUrl;
        }
        public AddressFields Profile { get; set; }
        public AmigoSkateShopParsedForm ParsedForm { get; set; }
        public string Email { get; }
        public string Size { get; set; }
        public string Captcha { get; set; }
        public string Instagram { get; set; }
        public string Shipping { get; set; }
        public string RaffleUrl { get; set; }
    }
}