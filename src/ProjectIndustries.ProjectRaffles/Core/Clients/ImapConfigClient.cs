using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
  public class ImapConfigClient : ApiClientBase, IImapConfigClient
  {
    public ImapConfigClient(ProjectIndustriesApiConfig apiConfig)
      : base(apiConfig)
    {
    }

    public Task<ImapConfig> LookupAsync(string domainName, string licenseKey, CancellationToken ct = default)
    {
      const string apiUrl = "/imap-config/lookup?domain={0}";
      return GetAsync<ImapConfig>(string.Format(apiUrl, Uri.EscapeUriString(domainName)), licenseKey, ct);
    }

    public async Task AddManualAsync(string domain, ImapConfig config, string licenseKey, CancellationToken ct = default)
    {
      const string apiUrl = "/imap-config/manual?domain={0}";
      await PostAsync(string.Format(apiUrl, Uri.EscapeUriString(domain)), licenseKey, config, ct);
    }
  }
}