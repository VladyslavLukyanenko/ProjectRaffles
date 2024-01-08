using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.SneakersDelightAccountGenerator
{
    public interface ISneakersDelightAccountGeneratorClient
    {
        void GenerateHttpClient();
        Task<SneakersDelightAccountGeneratorParsed> ParseLoginAsync(CancellationToken ct);

        Task<bool> SubmitAccountAsync(SneakersDelightAccountGeneratorParsed parsed, AddressFields addressFields, string email, string gender, string captcha,
            CancellationToken ct);
    }
}