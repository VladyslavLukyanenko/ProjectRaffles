using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public interface ISubmissionStatsEntryFactory
  {
    SubmissionStatsEntry CreateEntry(RaffleTask task);
  }
}