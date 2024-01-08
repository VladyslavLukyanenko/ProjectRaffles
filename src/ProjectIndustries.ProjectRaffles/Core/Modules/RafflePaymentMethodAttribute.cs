using System;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  [AttributeUsage(AttributeTargets.Class)]
  public class RafflePaymentMethodAttribute : Attribute
  {
    public RafflePaymentMethodAttribute(PaymentMethod paymentMethod)
    {
      PaymentMethod = paymentMethod;
    }

    public PaymentMethod PaymentMethod { get; }
  }
}