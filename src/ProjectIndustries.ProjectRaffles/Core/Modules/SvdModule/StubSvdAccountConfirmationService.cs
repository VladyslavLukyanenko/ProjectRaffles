using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SvdModule
{
  public class StubSvdAccountConfirmationService : ISvdAccountConfirmationService
  {
    public bool IsConfirmationMessage(Email email, IncomeMailMessage message)
    {
      return true;
    }

    public async Task ConfirmAsync(Email email, IncomeMailMessage message, CancellationToken ct = default)
    {
      await Task.Delay(TimeSpan.FromSeconds(1), ct);
    }
  }
}