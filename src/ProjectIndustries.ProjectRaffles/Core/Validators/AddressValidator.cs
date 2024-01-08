using FluentValidation;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Validators
{
  public class AddressValidator : AbstractValidator<Address>
  {
    public AddressValidator()
    {
      RuleFor(_ => _.City).NotEmpty();
      RuleFor(_ => _.AddressLine1).NotEmpty();
      RuleFor(_ => _.CountryId).NotEmpty();
      RuleFor(_ => _.FirstName).NotEmpty();
      RuleFor(_ => _.LastName).NotEmpty();
      RuleFor(_ => _.PhoneNumber).NotEmpty();
      RuleFor(_ => _.ZipCode).NotEmpty();
    }
  }
}