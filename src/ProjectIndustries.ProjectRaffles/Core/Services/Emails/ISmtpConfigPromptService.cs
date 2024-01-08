using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Emails
{
  public interface ISmtpConfigPromptService
  {
    Task<SmtpConfig> PromptAsync(Email email, CancellationToken ct = default);
  }
}