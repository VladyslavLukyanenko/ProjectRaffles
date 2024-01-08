using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
  public interface ISmtpConfigClient
  {
    Task<SmtpConfig> LookupAsync(string domainName, CancellationToken ct = default);
    Task AddManualAsync(string domain, SmtpConfig config, CancellationToken ct = default);
  }
}