using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.FenomAccountGenerator
{
    public interface IFenomAccountGeneratorClient
    {
        void GenerateHttpClient();
        Task<bool> SubmitAccount(AddressFields addressFields, string email, string gender);
    }
}