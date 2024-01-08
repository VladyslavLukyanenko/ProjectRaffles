using System;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Spatial;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public class RandomAddressResolver : IDynamicValueResolver, IValueChangePostProcessable
  {
    private AddressArea _area;
    private IAddressReverseService _reverseService;

    public RandomAddressResolver()
    {
      ResolveValue = async context =>
      {
        var loc = _area.GetNextRandomPointInArea();

        var location = await _reverseService.ReverseAddressAsync(loc);
        return location?.DisplayName;
      };
    }
    
    public string Name => "Random Address Picker";
    public Func<IRaffleExecutionContext, Task<string>> ResolveValue { get; }
    public async Task PostProcessAsync(IReadonlyDependencyResolver dependencyResolver)
    {
      var addressPicker = dependencyResolver.GetService<IAddressAreaPromptService>();
      _reverseService = dependencyResolver.GetService<IAddressReverseService>();

      _area = await addressPicker.PickAddressAreaAsync();
    }
  }
}