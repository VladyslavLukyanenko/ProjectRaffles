using System.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services.Modules;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public class DiBasedMailRaffleConfirmationHandlerProvider : IMailRaffleConfirmationHandlerProvider
  {
    private readonly IRaffleModulesProvider _modulesProvider;

    public DiBasedMailRaffleConfirmationHandlerProvider(IRaffleModulesProvider modulesProvider)
    {
      _modulesProvider = modulesProvider;
    }

    public IMailRaffleConfirmationHandler Get(string providerName)
    {
      var descriptor = _modulesProvider.SupportedModules.FirstOrDefault(_ => _.Name == providerName);
      if (descriptor == null)
      {
        return null;
      }

      return (IMailRaffleConfirmationHandler) Locator.Current.GetService(descriptor.MailConfirmationHandlerType);
    }
  }
}