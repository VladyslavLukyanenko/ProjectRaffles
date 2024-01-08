using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public interface IDeviceInfoProvider
    {
        Task<string> GetHwidAsync(CancellationToken ct);
    }
}