using ProjectIndustries.ProjectRaffles.Core.Services.Spatial;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker.AddressPicker
{
  public class RandomCityValueResolver : AddressPartPickerBase
  {
    public RandomCityValueResolver(IAddressPartResolutionProvider addressPartResolutionProvider)
      : base(addressPartResolutionProvider, "City", address => !string.IsNullOrEmpty(address.City))
    {
    }

    protected override string PickValueFromAddress(ReversedLocation location) =>
      location.Address.City ?? location.Address.SubUrb;
  }
}