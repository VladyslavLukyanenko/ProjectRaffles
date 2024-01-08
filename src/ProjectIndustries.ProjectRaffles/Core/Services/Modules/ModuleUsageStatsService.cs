using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Modules
{
  public class ModuleUsageStatsService : IModuleUsageStatsService
  {
    private readonly ILiteCollection<ModuleUsageStat> _stats;
    private static readonly SemaphoreSlim WriteGates = new SemaphoreSlim(10, 10);

    public ModuleUsageStatsService(ILiteDatabase database)
    {
      _stats = database.GetCollection<ModuleUsageStat>();
    }

    public Task<IList<ModuleUsageStat>> GetStatsAsync(string id, CancellationToken ct = default)
    {
      IList<ModuleUsageStat> stats = _stats.Find(_ => _.ModuleComputedId == id).ToList();
      return Task.FromResult(stats);
    }

    public Task CreateFromTaskAsync(RaffleTask task, CancellationToken ct = default)
    {
      try
      {
        WriteGates.Wait(ct);
        var stat = new ModuleUsageStat
        {
          ProfileId = task.Profile?.Id ?? Guid.Empty,
          ProxyId = task.Proxy?.Id,
          ModuleComputedId = task.Module.ComputedIdentity
        };

        _stats.Insert(stat);
      }
      finally
      {
        if (!ct.IsCancellationRequested)
        {
          WriteGates.Release();
        }
      }

      return Task.CompletedTask;
    }
  }
}