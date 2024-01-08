using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NotreModule
{
    [EnableCaching]
    public interface INotreClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<NotreParsed> ParseNotreAsync(CancellationToken ct);

        Task SetXsrf(CancellationToken ct);

        Task<NotreQuestion> GetCaptchaQuestionAsync(CancellationToken ct);

        [CacheOutput]
        Task<string> GetCaptchaAnswerAsync(string question, CancellationToken ct);

        Task<NotreQuestion> ValidateAnswerAsync(NotreQuestion question, string answer);

        Task RegisterUser(AddressFields addressFields, NotreQuestion question, string userEmail, CancellationToken ct);

        Task<bool> SubmitEntryToRaffleAsync(NotreParsed parsed, int raffle, string userSize,
            CancellationToken ct);
    }
}