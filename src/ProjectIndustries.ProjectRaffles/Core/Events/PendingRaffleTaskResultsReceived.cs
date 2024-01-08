using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Events
{
  public class PendingRaffleTaskResultsReceived
  {
    public PendingRaffleTaskResultsReceived(PendingRaffleTask task)
    {
      Task = task;
    }

    public PendingRaffleTask Task { get; private set; }
  }
}