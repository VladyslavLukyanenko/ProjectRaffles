using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.GoogleFormsModule
{
    public interface IGoogleFormsClient : IModuleHttpClient
    {
        Task<bool> SubmitAsync(List<KeyValuePair<string,string>> list, string sourceUrl, CancellationToken ct);
    }
}