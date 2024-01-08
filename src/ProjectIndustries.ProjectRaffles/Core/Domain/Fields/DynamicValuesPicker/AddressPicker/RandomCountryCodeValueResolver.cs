using ProjectIndustries.ProjectRaffles.Core.Services.Spatial;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker.AddressPicker
{
  public class RandomCountryCodeValueResolver : AddressPartPickerBase
  {
    public RandomCountryCodeValueResolver(IAddressPartResolutionProvider addressPartResolutionProvider)
      : base(addressPartResolutionProvider, "Country Code", _ => !string.IsNullOrEmpty(_.CountryCode))
    {
    }

    protected override string PickValueFromAddress(ReversedLocation location) => location.Address.CountryCode;
  }
}