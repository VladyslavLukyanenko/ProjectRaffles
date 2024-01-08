using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TsumModule
{
    public class TsumSubmitPayload
    {
        public TsumSubmitPayload(AddressFields profile, string email, TsumProductTags productTags, string pickup, string size, string gender, string age)
        {
            Profile = profile;
            Email = email;
            ProductTags = productTags;
            PickupLocation = pickup;
            Size = size;
            Gender = gender;
            Age = age;
        }
        public AddressFields Profile { get; }
        public string Email { get; }
        public TsumProductTags ProductTags { get; }
        public string PickupLocation { get; }
        public string Size { get; }
        public string Gender { get; }
        public string Age { get; }
    }
}