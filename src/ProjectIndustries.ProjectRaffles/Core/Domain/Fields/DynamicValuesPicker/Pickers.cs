using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker.AddressPicker;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public static class Pickers
  {
    static Pickers()
    {
      ProfileFields = new ProfileValuesGroup();
      ProfileShippingAddressFields = new ShippingAddressValuesGroup();
      ProfileCreditCardFields = new CreditCardValuesGroup();
      Misc = new MiscDynamicValuesGroup();
    }

    public static ProfileValuesGroup ProfileFields { get; }
    public static ShippingAddressValuesGroup ProfileShippingAddressFields { get; }
    public static CreditCardValuesGroup ProfileCreditCardFields { get; }
    public static MiscDynamicValuesGroup Misc { get; }

    public static IDynamicValuesGroup[] All => new IDynamicValuesGroup[]
    {
      ProfileFields,
      ProfileShippingAddressFields,
      ProfileCreditCardFields,
      Misc,
      AddressPickerValuesGroup.CreateOptimized()
    };
  }
}