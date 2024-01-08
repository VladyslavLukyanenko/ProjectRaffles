using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Emails
{
  public class SmtpValidationService : ISmtpValidationService
  {
    public async Task<bool> IsValidAsync(Email email, SmtpConfig config, CancellationToken ct = default)
    {
      try
      {
        var client = new SmtpClient();
        await client.ConnectAsync(config.Host, config.Port, SecureSocketOptions.Auto, ct);
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