using System.Collections.Generic;
using DynamicData;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Tasks
{
  public interface ITasksService
  {
    IObservableCache<RaffleTask, long> Tasks { get; }
    void Add(RaffleTask task);
    void AddRange(IEnumerable<RaffleTask> tasks);
    void Remove(RaffleTask task);
    void Clear();
  }
}