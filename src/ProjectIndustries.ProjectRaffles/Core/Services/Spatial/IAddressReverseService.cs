using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.WpfUI.MapBox.MapboxNetCore;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Spatial
{
  public interface IAddressReverseService
  {
    Task<ReversedLocation> ReverseAddressAsync(GeoLocation point, CancellationToken ct = default);
  }
}