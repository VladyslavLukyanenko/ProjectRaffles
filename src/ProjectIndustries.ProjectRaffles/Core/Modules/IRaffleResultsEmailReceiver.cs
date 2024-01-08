using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public interface IRaffleResultsEmailReceiver
  {
    Email ReceiverEmail { get; }
  }
}