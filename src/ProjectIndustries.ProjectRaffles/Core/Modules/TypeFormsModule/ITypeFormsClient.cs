using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TypeFormsModule
{
  public interface ITypeFormsClient : IModuleHttpClient
  {
    Task<TypeFormsSubmission> StartSubmissionAsync(string url, CancellationToken ct);
    Task<bool> SubmitAsync(string sourceUrl, TypeFormsSubmitPayload payload, CancellationToken ct);
  }
}