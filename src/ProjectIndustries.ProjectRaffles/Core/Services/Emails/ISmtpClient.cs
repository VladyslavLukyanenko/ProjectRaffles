using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Emails
{
  public interface ISmtpClient
  {
    Task SendAsync(string receiverEmail, string subject, string text, Email email, CancellationToken ct = default);
  }
}