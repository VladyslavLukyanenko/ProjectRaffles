using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Emails
{
  public interface IImapConfigPromptService
  {
    Task<ImapConfig> PromptAsync(Email email, CancellationToken ct = default);
  }
}