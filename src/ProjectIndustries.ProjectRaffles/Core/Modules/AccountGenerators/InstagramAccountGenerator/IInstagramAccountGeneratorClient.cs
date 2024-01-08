using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.InstagramAccountGenerator
{
    public interface IInstagramAccountGeneratorClient
    {
        void GenerateInstaApiHandler(string email, Proxy proxy);

        Task CheckEmailAvailability(string email);

        Task<bool> CreateAccount(AddressFields nameValues, string email, CancellationToken ct);
    }
}