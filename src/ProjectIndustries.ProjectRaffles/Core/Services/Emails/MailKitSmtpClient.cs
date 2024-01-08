using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Emails
{
  public class MailKitSmtpClient : ISmtpClient
  {
    public async Task SendAsync(string receiverEmail, string subject, string text, Email email,
      CancellationToken ct = default)
    {
      using (var smtp = new SmtpClient(new ProtocolLogger(Console.OpenStandardOutput(), true)))
      {
        var cfg = email.SmtpConfig;
        SecureSocketOptions socketOptions = SecureSocketOptions.Auto;
        await smtp.ConnectAsync(cfg.Host, cfg.Port, socketOptions, ct);

        var credentials = new SaslMechanismLogin(email.Value, email.Password);
        await smtp.AuthenticateAsync(credentials, ct);

        var message = new MimeMessage
        {
          Subject = subject,
          Body = new TextPart("plain")
          {
            Text = text
          }
        };

        message.From.Add(new MailboxAddress(email.Value, email.Value));
        message.To.Add(new MailboxAddress(receiverEmail, receiverEmail));
        await smtp.SendAsync(message, ct);
        await smtp.DisconnectAsync(true, ct);
      }
    }
  }
}