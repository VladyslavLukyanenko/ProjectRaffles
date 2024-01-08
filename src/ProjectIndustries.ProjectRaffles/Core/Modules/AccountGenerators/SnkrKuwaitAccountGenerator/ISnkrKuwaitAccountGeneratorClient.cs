using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.SnkrKuwaitAccountGenerator
{
    public interface ISnkrKuwaitAccountGeneratorClient
    {
        void GenerateHttpClient();
        Task<bool> SubmitAccount(AddressFields addressFields, string email, CancellationToken ct);
    }
}