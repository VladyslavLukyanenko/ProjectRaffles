using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.BrownsFashionAccountGenerator
{
    public interface IBrownsFashionAccountGeneratorClient
    {
        void GenerateHttpClient();
        Task<bool> SubmitAccount(AddressFields addressFields, string email, CancellationToken ct);
    }
}