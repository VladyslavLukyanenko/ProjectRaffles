using System;
using ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  [AttributeUsage(AttributeTargets.Class)]
  public class MailRaffleStatusExtractorAttribute : Attribute
  {
    public MailRaffleStatusExtractorAttribute(Type extractorType)
    {
      if (!typeof(IMailRaffleStatusExtractor).IsAssignableFrom(extractorType))
      {
        throw new ArgumentException($"Provided type mush implement '{typeof(IMailRaffleStatusExtractor)}' interface");
      }

      ExtractorType = extractorType;
    }

    public Type ExtractorType { get; }
  }
}