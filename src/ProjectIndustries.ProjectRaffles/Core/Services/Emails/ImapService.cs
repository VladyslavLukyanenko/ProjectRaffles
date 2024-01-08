using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using ProjectIndustries.ProjectRaffles.Core.Clients;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Emails
{
  public class ImapService : IImapService
  {
    private readonly ILicenseKeyProvider _licenseKeyProvider;
    private readonly IImapConfigClient _imapClient;

    public ImapService(ILicenseKeyProvider licenseKeyProvider, IImapConfigClient imapClient)
    {
      _licenseKeyProvider = licenseKeyProvider;
      _imapClient = imapClient;
    }

    public Task<ImapConfig> LookupAsync(string domainName, CancellationToken ct = default)
    {
      return _imapClient.LookupAsync(domainName, _licenseKeyProvider.CurrentLicenseKey, ct);
    }

    public Task AddManualAsync(string domain, ImapConfig config, CancellationToken ct = default)
    {
      return _imapClient.AddManualAsync(domain, config, _licenseKeyProvider.CurrentLicenseKey, ct);
    }

    public async Task<bool> IsValidAsync(Email email, ImapConfig config, CancellationToken ct = default)
    {
      try
      {
        var client = new ImapClient();
        await client.ConnectAsync(config.Host, config.Port, config.UseSsl, ct);
        await client.AuthenticateAsync(email.Value, email.Password, ct);
        await client.DisconnectAsync(true, ct);

        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}