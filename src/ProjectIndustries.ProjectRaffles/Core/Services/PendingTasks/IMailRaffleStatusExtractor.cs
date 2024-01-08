namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public interface IMailRaffleStatusExtractor
  {
    bool IsExpectedMail(IncomeMailMessage message);
    bool IsWinner(IncomeMailMessage message);
  }
}