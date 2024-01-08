using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services.PaymentProcessors;

namespace ProjectIndustries.ProjectRaffles.Core.Validators
{
  public class ProfileValidator : AbstractValidator<Profile>
  {
    private readonly CreditCardValidator _creditCardValidator;

    public ProfileValidator(AddressValidator addressValidator, CreditCardValidator creditCardValidator)
    {
      _creditCardValidator = creditCardValidator;
      RuleFor(_ => _.ProfileName).NotEmpty();
      RuleFor(_ => _.BillingAddress).Must((p, a) => p.IsShippingSameAsBilling || addressValidator.Validate(a).IsValid);
      RuleFor(_ => _.ShippingAddress).SetValidator(addressValidator);
      RuleFor(_ => _.CreditCard).MustAsync(BeValidOrNull);
    }

    private async Task<bool> BeValidOrNull(CreditCard card, CancellationToken ct)
    {
      return card == null || (await _creditCardValidator.ValidateAsync(card, ct)).IsValid;
    }
  }
}