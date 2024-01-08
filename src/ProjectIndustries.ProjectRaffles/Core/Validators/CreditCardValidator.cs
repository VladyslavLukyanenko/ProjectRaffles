using FluentValidation;

using ProjectIndustries.ProjectRaffles.Core.Services.PaymentProcessors;

using System;

namespace ProjectIndustries.ProjectRaffles.Core.Validators
{
  public class CreditCardValidator : AbstractValidator<CreditCard>
  {
    public CreditCardValidator()
    {
      RuleFor(_ => _.Cvv).NotEmpty().Matches(@"\d{3,4}");
      RuleFor(_ => _.ExpirationMonth).InclusiveBetween(1, 12);
      RuleFor(_ => _.ExpirationYear).GreaterThanOrEqualTo(DateTime.Now.Year);
      RuleFor(_ => _.Number).CreditCard().NotEmpty();
    }
  }
}
