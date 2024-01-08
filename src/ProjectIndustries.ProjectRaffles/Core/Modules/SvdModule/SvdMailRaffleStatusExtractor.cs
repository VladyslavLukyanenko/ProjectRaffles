using ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SvdModule
{
  public class SvdMailRaffleStatusExtractor : IMailRaffleStatusExtractor
  {
    public bool IsExpectedMail(IncomeMailMessage message)
    {
      return message.Subject.Contains("ProjectIndustriesSvdTest");
    }

    public bool IsWinner(IncomeMailMessage message)
    {
      return message.Subject.Contains("Win");
    }
  }
}