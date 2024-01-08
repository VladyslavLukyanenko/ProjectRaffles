using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Emails
{
  public interface ISmtpValidationService
  {
    Task<bool> IsValidAsync(Email email, SmtpConfig config, CancellationToken ct = default);
  }
}