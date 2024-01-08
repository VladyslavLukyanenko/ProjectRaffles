namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public class RandomFirstNameValueResolver : RandomFakeNameValueResolverBase
  {
    public RandomFirstNameValueResolver()
      : base("Random First Name", "Configure First Name Picker")
    {
    }

    protected override string GetNonUniqueRandomValue()
    {
      return Faker.Name.FirstName(GendersSelect.Value);
    }
  }
}