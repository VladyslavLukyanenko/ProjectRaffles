namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public class RandomPhoneNumberValueResolver : FakerRandomValueResolverBase
  {
    public RandomPhoneNumberValueResolver()
      : base("Random Phone Number", "Configure Phone Number Picker")
    {
    }

    protected override string GetNonUniqueRandomValue()
    {
      return Faker.Phone.PhoneNumberFormat();
    }
  }
}