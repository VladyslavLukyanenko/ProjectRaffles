using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Services.Spatial;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker.AddressPicker
{
  public interface IAddressPartResolutionProvider
  {
    Task<ReversedLocation> GetAddressAsync();
    Task InitializeAreaAsync(IReadonlyDependencyResolver dependencyResolver);
  }
}