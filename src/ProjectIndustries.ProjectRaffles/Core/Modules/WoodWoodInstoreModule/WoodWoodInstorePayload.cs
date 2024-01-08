using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WoodWoodInstoreModule
{
    public class WoodWoodInstorePayload
    {
        public WoodWoodInstorePayload(AddressFields address, string email, string raffletag, string captcha, string ip, string phonecode, string store, string size)
        {
            Address = address;
            Email = email;
            ProductTag = raffletag;
            Captcha = captcha;
            Ip = ip;
            PhoneCode = phonecode;
            Store = store;
            Size = size;
        }
        public AddressFields Address { get; }
        public string Email { get; }
        public string ProductTag { get; }
        public string Captcha { get; }
        public string Ip { get; }
        public string PhoneCode { get; }
        public string Store { get; }
        public string Size { get; }
    }
}