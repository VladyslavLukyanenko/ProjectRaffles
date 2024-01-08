using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DelibertiModule
{
    public interface IDelibertiClient : IModuleHttpClient
    {
        Task<bool> SubmitAsync(AddressFields profile, string email, string size, string streetnumber, string region,
            string instagram, string raffleurl, CancellationToken ct);
    }
}