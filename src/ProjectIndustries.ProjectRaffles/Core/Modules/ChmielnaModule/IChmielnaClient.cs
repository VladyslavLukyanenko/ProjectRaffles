using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.ChmielnaModule
{
    public interface IChmielnaClient : IModuleHttpClient
    {
        Task<string> GetProductAsync(string raffleurl, CancellationToken ct);
        Task<string> GetCountryCode(AddressFields profile, CancellationToken ct);
        Task<bool> SubmitAsync(AddressFields profile, string email, string raffleurl, string countryCode, string captcha, string size, CancellationToken ct);
    }
}