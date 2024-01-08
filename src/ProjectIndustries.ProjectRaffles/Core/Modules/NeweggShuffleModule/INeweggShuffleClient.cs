using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.NeweggAccountGenerator;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NeweggShuffleModule
{
    public interface INeweggShuffleClient : IModuleHttpClient
    {
        Task LoginWithCookies(Account account, CancellationToken ct);
        Task<string> GetTicketId(CancellationToken ct);
        Task<NeweggFormValues> ParseData(string ticketKey, CancellationToken ct);
        Task LoginWithEmail(Account account, NeweggFormValues form, string ticketId, CancellationToken ct);

        [CacheOutput]
        Task<string> GetLotteryId(CancellationToken ct);

        Task<bool> SignupToShuffle(string shuffleId, string lotteryId, CancellationToken ct);
    }
}