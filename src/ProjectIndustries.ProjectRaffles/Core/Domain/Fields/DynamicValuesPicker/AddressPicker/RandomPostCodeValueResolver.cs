using ProjectIndustries.ProjectRaffles.Core.Services.Spatial;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker.AddressPicker
{
  public class RandomPostCodeValueResolver : AddressPartPickerBase
  {
    public RandomPostCodeValueResolver(IAddressPartResolutionProvider addressPartResolutionProvider)
      : base(addressPartResolutionProvider, "Post Code",
        _ => !string.IsNullOrEmpty(_.PostCode) && !_.PostCode.Contains("-"))
    {
    }

    protected override string PickValueFromAddress(ReversedLocation location) => location.Address.PostCode;
  }
}