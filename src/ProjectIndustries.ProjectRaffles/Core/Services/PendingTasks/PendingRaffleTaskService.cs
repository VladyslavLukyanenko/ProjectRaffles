using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public class PendingRaffleTaskService : LiteDbRepositoryBase<PendingRaffleTask>, IPendingRaffleTaskService
  {
    public PendingRaffleTaskService(ILiteDatabase database)
      : base(database)
    {
    }

    public Task<IList<PendingRaffleTask>> GetPendingAsync(CancellationToken ct = default)
    {
      IList<PendingRaffleTask> pending = Collection.Find(_ => !_.NotifiedAt.HasValue).ToList();
      return Task.FromResult(pending);
    }

    public Task<IList<PendingRaffleTask>> GetExpiredTasksAsync(CancellationToken ct = default)
    {
      IList<PendingRaffleTask> expired =
        Collection.Find(_ => !_.NotifiedAt.HasValue && _.ExpiresAt < DateTimeOffset.Now).ToList();

      return Task.FromResult(expired);
    }
  }
}