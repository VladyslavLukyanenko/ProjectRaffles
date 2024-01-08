using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public interface IMasterEmailPromptService
  {
    Task<Email> PromptAsync(CancellationToken ct = default);
  }
}