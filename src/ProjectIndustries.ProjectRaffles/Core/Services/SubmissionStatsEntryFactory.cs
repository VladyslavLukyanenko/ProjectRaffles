using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public class SubmissionStatsEntryFactory : ISubmissionStatsEntryFactory
  {
    public SubmissionStatsEntry CreateEntry(RaffleTask task) => new SubmissionStatsEntry
    {
      Sizes = task.Size,
      IsSuccessful = task.Status.Kind == RaffleStatusKind.Succeeded,
      ProductName = task.ProductName,
      ProfileName = task.Profile?.ProfileName ?? "<No profile>",
      ProviderName = task.ProviderName
    };
  }
}