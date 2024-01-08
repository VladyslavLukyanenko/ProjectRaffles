using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.NeweggAccountGenerator
{
    public interface INeweggAccountGeneratorClient
    {
        void GenerateHttpClient();

        Task<string> GetTicketId(CancellationToken ct);

        Task<NeweggFormValues> ParseData(string ticketId, CancellationToken ct);

        Task<dynamic> SubmitAccount(AddressFields addressFields, string email, string captcha,
            NeweggFormValues formValues, string ticketId, CancellationToken ct);

        Task<string> GetAccountCookies(dynamic response, CancellationToken ct);
    }
}