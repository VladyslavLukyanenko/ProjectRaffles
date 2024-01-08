namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public interface IMailRaffleConfirmationHandlerProvider
  {
    IMailRaffleConfirmationHandler Get(string providerName);
  }
}