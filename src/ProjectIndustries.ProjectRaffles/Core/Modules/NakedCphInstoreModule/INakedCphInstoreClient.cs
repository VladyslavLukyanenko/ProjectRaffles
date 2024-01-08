using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NakedCphInstoreModule
{
    public interface INakedCphInstoreClient : IModuleHttpClient
    {
        Task<bool> SubmitAsync(AddressFields addressFields, string email, string instagramHandle, string raffleTag,
            string size, CancellationToken ct);
    }
}