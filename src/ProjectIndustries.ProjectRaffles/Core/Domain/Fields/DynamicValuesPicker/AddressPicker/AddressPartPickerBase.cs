using System;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Spatial;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker.AddressPicker
{
  public abstract class AddressPartPickerBase : IDynamicValueResolver, IValueChangePostProcessable
  {
    private IAddressPartResolutionProvider _addressPartResolutionProvider;

    protected AddressPartPickerBase(IAddressPartResolutionProvider addressPartResolutionProvider, string name,
      Predicate<ReversedAddress> isValid)
    {
      _addressPartResolutionProvider = addressPartResolutionProvider;
      Name = name;
      IsValid = isValid;
      ResolveValue = ResolveValueAsync;
    }

    public string Name { get; }
    public Predicate<ReversedAddress> IsValid { get; }
    public Func<IRaffleExecutionContext, Task<string>> ResolveValue { get; private set; }

    public Task PostProcessAsync(IReadonlyDependencyResolver dependencyResolver)
    {
      return _addressPartResolutionProvider.InitializeAreaAsync(dependencyResolver);
    }

    public AddressPartPickerBase CreateClone(IAddressPartResolutionProvider provider)
    {
      var picker = (AddressPartPickerBase) MemberwiseClone();
      picker._addressPartResolutionProvider = provider;
      picker.ResolveValue = picker.ResolveValueAsync;

      return picker;
    }

    protected abstract string PickValueFromAddress(ReversedLocation location);

    private async Task<string> ResolveValueAsync(IRaffleExecutionContext ctx)
    {
      var address = await _addressPartResolutionProvider.GetAddressAsync();

      return PickValueFromAddress(address);
    }
  }
}