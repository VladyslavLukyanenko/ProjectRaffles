using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Emails
{
  public interface IImapService
  {
    Task<ImapConfig> LookupAsync(string domainName, CancellationToken ct = default);
    Task AddManualAsync(string domain, ImapConfig config, CancellationToken ct = default);
    Task<bool> IsValidAsync(Email email, ImapConfig config, CancellationToken ct = default);
  }
}