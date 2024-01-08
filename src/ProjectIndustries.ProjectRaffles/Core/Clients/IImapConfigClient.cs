using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
  public interface IImapConfigClient
  {
    Task<ImapConfig> LookupAsync(string domainName, string licenseKey, CancellationToken ct = default);
    Task AddManualAsync(string domain, ImapConfig config, string licenseKey, CancellationToken ct = default);
  }
}