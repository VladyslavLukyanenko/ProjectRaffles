using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp
{
  public static class MailChimpFieldValueResolverFactory
  {
    public static IDynamicValueResolver GetForAddressPart(string part) => part switch
    {
      "addr1" => Pickers.ProfileShippingAddressFields.AddressLine1,
      "addr2" => Pickers.ProfileShippingAddressFields.AddressLine2,
      "city" => Pickers.ProfileShippingAddressFields.City,
      "state" => Pickers.ProfileShippingAddressFields.ProvinceCode,
      "zip" => Pickers.ProfileShippingAddressFields.ZipCode,
      _ => null
    };

    public static IDynamicValueResolver GetForRegularField(string part) => part switch
    {
      "fname" => Pickers.ProfileFields.FirstName,
      "lname" => Pickers.ProfileFields.LastName,
      "email" => Pickers.Misc.Email,
      "phone" => Pickers.ProfileShippingAddressFields.PhoneNumber,
      _ => null
    };
  }
}