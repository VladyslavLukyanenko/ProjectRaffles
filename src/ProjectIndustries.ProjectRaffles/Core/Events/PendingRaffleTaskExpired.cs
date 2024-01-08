using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Events
{
  public class PendingRaffleTaskExpired
  {
    public PendingRaffleTaskExpired(PendingRaffleTask task)
    {
      Task = task;
    }

    public PendingRaffleTask Task { get; }
  }
}