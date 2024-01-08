using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
    public interface ILicenseKeyApiClient
    {
        Task<AuthenticationResult> Authenticate(string licenseKey, string hwid, CancellationToken ct = default);
        Task DeactivateOnCurrentDeviceAsync(string licenseKey, CancellationToken ct = default);
    }
}