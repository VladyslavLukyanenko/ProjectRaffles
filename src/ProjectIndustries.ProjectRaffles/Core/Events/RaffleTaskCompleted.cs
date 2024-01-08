using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Events
{
  public class RaffleTaskCompleted
  {
    public RaffleTaskCompleted(RaffleTask task)
    {
      Task = task;
    }

    public RaffleTask Task { get; }
  }
}