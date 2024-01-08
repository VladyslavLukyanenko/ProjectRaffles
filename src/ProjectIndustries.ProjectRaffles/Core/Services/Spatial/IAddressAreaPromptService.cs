using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Spatial
{
  public interface IAddressAreaPromptService
  {
    Task<AddressArea> PickAddressAreaAsync(CancellationToken ct = default);
  }
}