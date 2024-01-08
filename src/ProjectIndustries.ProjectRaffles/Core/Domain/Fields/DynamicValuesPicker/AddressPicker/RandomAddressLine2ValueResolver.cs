using System;
using System.Globalization;
using ProjectIndustries.ProjectRaffles.Core.Services.Spatial;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker.AddressPicker
{
  public class RandomAddressLine2ValueResolver : AddressPartPickerBase
  {
    private static readonly Random Random = new Random((int) DateTime.Now.Ticks);
    private const int MinNum = 1;
    private const int MaxNum = 100;
    public RandomAddressLine2ValueResolver(IAddressPartResolutionProvider addressPartResolutionProvider)
      : base(addressPartResolutionProvider, "Address Line 2", _ => true)
    {
    }

    protected override string PickValueFromAddress(ReversedLocation location)
    {
      return Random.Next(MinNum, MaxNum + 1).ToString(CultureInfo.InvariantCulture);
    }
  }
}