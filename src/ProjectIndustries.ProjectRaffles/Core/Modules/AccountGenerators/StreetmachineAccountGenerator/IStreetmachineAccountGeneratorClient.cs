using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.StreetmachineAccountGenerator
{
    public interface IStreetmachineAccountGeneratorClient
    {
        void GenerateHttpClient();
        Task GetCookies(CancellationToken ct);
        Task<bool> SubmitAccountAsync(string email, AddressFields addressFields, string captcha, CancellationToken ct);
    }
}