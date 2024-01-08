namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public interface IMailRaffleStatusExtractorProvider
  {
    IMailRaffleStatusExtractor Get(string providerName);
  }
}