using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public interface IMailRaffleConfirmationHandler
  {
    bool IsExpectedMail(IncomeMailMessage message);
    Task ConfirmAsync(IncomeMailMessage message, CancellationToken ct = default);
  }
}