using System;
using ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  [AttributeUsage(AttributeTargets.Class)]
  public class MailRaffleConfirmationHandlerAttribute : Attribute
  {
    public MailRaffleConfirmationHandlerAttribute(Type extractorType)
    {
      if (!typeof(IMailRaffleConfirmationHandler).IsAssignableFrom(extractorType))
      {
        throw new ArgumentException($"Provided type mush implement '{typeof(IMailRaffleConfirmationHandler)}' interface");
      }

      ExtractorType = extractorType;
    }

    public Type ExtractorType { get; }
  }
}