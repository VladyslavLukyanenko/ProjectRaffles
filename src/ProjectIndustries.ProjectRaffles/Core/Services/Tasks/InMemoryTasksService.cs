using System.Collections.Generic;
using DynamicData;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Tasks
{
  public class InMemoryTasksService : ITasksService
  {
    private readonly SourceCache<RaffleTask, long> _tasks =
      new SourceCache<RaffleTask, long>(_ => _.Id);

    public InMemoryTasksService()
    {
      Tasks = _tasks.AsObservableCache();
    }

    public IObservableCache<RaffleTask, long> Tasks { get; }

    public void Add(RaffleTask task)
    {
      _tasks.AddOrUpdate(task);
    }

    public void AddRange(IEnumerable<RaffleTask> tasks)
    {
      _tasks.AddOrUpdate(tasks);
    }

    public void Remove(RaffleTask task)
    {
      _tasks.Remove(task);
    }

    public void Clear()
    {
      _tasks.Clear();
    }
  }
}