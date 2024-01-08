using System.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.ShinzoParisModule
{
    public class ShinzoParisSubmitPayload
    {
        public ShinzoParisSubmitPayload(AddressFields profile, ShinzoParisParsedRaffle parsedRaffle, string shippingType, string size, string instagram, string captcha, string email)
        {
            Profile = profile;
            ParsedRaffle = parsedRaffle;
            ShippingType = shippingType;
            Size = size;
            Instagram = instagram;
            Captcha = captcha;
            Email = email;
        }
        
        public AddressFields Profile { get; }
        public ShinzoParisParsedRaffle ParsedRaffle { get; }
        public string ShippingType { get; }
        public string Size { get; }
        public string Instagram { get; }
        public string Captcha { get; }
        public string Email { get; }
    }
}