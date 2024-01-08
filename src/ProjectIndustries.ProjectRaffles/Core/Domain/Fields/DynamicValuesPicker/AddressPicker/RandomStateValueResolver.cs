using ProjectIndustries.ProjectRaffles.Core.Services.Spatial;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker.AddressPicker
{
  public class RandomStateValueResolver : AddressPartPickerBase
  {
    public RandomStateValueResolver(IAddressPartResolutionProvider addressPartResolutionProvider)
      : base(addressPartResolutionProvider, "State", _ => !string.IsNullOrEmpty(_.State))
    {
    }

    protected override string PickValueFromAddress(ReversedLocation location) => location.Address.State;
  }
}