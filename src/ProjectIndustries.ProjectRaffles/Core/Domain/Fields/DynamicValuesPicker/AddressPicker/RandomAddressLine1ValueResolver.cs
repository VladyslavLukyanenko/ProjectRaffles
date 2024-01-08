using ProjectIndustries.ProjectRaffles.Core.Services.Spatial;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker.AddressPicker
{
  public class RandomAddressLine1ValueResolver : AddressPartPickerBase
  {
    public RandomAddressLine1ValueResolver(IAddressPartResolutionProvider addressPartResolutionProvider)
      : base(addressPartResolutionProvider, "Address Line 1",
        _ => !string.IsNullOrEmpty(_.Road) && !string.IsNullOrEmpty(_.HouseNumber))
    {
    }

    protected override string PickValueFromAddress(ReversedLocation location)
    {
      return $"{location.Address.Road}, {location.Address.HouseNumber}";
    }
  }
}