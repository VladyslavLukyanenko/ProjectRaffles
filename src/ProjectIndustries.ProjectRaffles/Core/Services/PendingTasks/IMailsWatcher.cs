using System.Collections.Generic;
using System.Threading;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public interface IMailsWatcher
  {
    IAsyncEnumerable<IncomeMailMessage> WatchAsync(Email email, uint? startMailId = null,
      CancellationToken ct = default);
  }
}