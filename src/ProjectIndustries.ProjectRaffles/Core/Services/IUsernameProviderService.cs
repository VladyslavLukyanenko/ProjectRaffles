using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public interface IUsernameProviderService
    {
        Task<string> GenerateUsernameAsync(CancellationToken ct);
    }
}