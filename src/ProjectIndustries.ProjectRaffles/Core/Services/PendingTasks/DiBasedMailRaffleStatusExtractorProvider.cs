using System;
using System.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services.Modules;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public class DiBasedMailRaffleStatusExtractorProvider : IMailRaffleStatusExtractorProvider
  {
    private readonly IRaffleModulesProvider _modulesProvider;

    public DiBasedMailRaffleStatusExtractorProvider(IRaffleModulesProvider modulesProvider)
    {
      _modulesProvider = modulesProvider;
    }

    public IMailRaffleStatusExtractor Get(string providerName)
    {
      var descriptor = _modulesProvider.SupportedModules.FirstOrDefault(_ => _.Name == providerName);
      if (descriptor == null)
      {
        throw new InvalidOperationException("Can't find descriptor for module " + providerName);
      }

      var extractor = (IMailRaffleStatusExtractor) Locator.Current.GetService(descriptor.MailResultsExtractorType);
      if (descriptor == null)
      {
        throw new InvalidOperationException("Can't resolve results extractor for module " + providerName);
      }
      
      return extractor;
    }
  }
}