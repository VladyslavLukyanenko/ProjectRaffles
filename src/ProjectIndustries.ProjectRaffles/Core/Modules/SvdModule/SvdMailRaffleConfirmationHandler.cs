using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SvdModule
{
  public class SvdMailRaffleConfirmationHandler : IMailRaffleConfirmationHandler
  {
    public bool IsExpectedMail(IncomeMailMessage message)
    {
      return true;
    }

    public Task ConfirmAsync(IncomeMailMessage message, CancellationToken ct = default)
    {
      return Task.CompletedTask;
    }
  }
}