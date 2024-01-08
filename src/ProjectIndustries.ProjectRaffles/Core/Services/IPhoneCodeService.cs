using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public interface IPhoneCodeService
    {
        Task<string> GetPhoneCodeAsync(string countryCode, CancellationToken ct);
    }
}