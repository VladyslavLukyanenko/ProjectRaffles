using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
  public class SmtpConfigClient : ApiClientBase, ISmtpConfigClient
  {
    private readonly ILicenseKeyProvider _licenseKeyProvider;

    public SmtpConfigClient(ProjectIndustriesApiConfig apiConfig, ILicenseKeyProvider licenseKeyProvider)
      : base(apiConfig)
    {
      _licenseKeyProvider = licenseKeyProvider;
    }

    public Task<SmtpConfig> LookupAsync(string domainName, CancellationToken ct = default)
    {
      const string apiUrl = "/smtp-config/lookup?domain={0}";
      return GetAsync<SmtpConfig>(string.Format(apiUrl, Uri.EscapeUriString(domainName)),
        _licenseKeyProvider.CurrentLicenseKey, ct);
    }

    public async Task AddManualAsync(string domain, SmtpConfig config, CancellationToken ct = default)
    {
      const string apiUrl = "/smtp-config/manual?domain={0}";
      await PostAsync(string.Format(apiUrl, Uri.EscapeUriString(domain)), _licenseKeyProvider.CurrentLicenseKey, config,
        ct);
    }
  }
}