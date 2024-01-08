using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public class DiBasedPendingRaffleTaskStatusWatcherFactory : IPendingRaffleTaskStatusWatcherFactory
  {
    public IPendingRaffleTaskStatusWatcher Create()
    {
      return Locator.Current.GetService<IPendingRaffleTaskStatusWatcher>();
    }
  }
}