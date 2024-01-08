using System;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Modules
{
  public class RaffleModuleDescriptor
  {
    public RaffleModuleDescriptor(string name, Type moduleType, bool requiresCaptcha, bool requiresCreditCard,
      Type mailResultsExtractorType, Type mailConfirmationHandlerType, Type accountGeneratorType, RaffleModuleType type,
      bool noProfileRequires)
    {
      Name = name;
      ModuleType = moduleType;
      RequiresCaptcha = requiresCaptcha;
      RequiresCreditCard = requiresCreditCard;
      MailResultsExtractorType = mailResultsExtractorType;
      MailConfirmationHandlerType = mailConfirmationHandlerType;
      AccountGeneratorType = accountGeneratorType;
      Type = type;
      NoProfileRequires = noProfileRequires;
    }

    public string Name { get; }
    public Type ModuleType { get; }
    public Type MailResultsExtractorType { get; }
    public Type MailConfirmationHandlerType { get; }
    public Type AccountGeneratorType { get; }
    public bool RequiresCaptcha { get; }
    public bool RequiresCreditCard { get; }
    public RaffleModuleType Type { get; }
    public bool NoProfileRequires { get; }
  }
}