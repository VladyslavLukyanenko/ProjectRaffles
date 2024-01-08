using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SvdModule
{
  public interface ISvdAccountConfirmationService
  {
    bool IsConfirmationMessage(Email email, IncomeMailMessage message);
    Task ConfirmAsync(Email email, IncomeMailMessage message, CancellationToken ct = default);
  }
}