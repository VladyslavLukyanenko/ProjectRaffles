using System;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  [AttributeUsage(AttributeTargets.Class)]
  public class RafflePaymentProcessorTypeAttribute : Attribute
  {
    public RafflePaymentProcessorTypeAttribute(PaymentProcessorType type)
    {
      Type = type;
    }

    public PaymentProcessorType Type { get; }
  }
}