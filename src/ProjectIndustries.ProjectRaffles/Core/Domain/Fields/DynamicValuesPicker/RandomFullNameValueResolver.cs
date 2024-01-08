namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public class RandomFullNameValueResolver : RandomFakeNameValueResolverBase
  {
    public RandomFullNameValueResolver()
      : base("Random Full Name", "Configure Full Name Picker")
    {
    }

    protected override string GetNonUniqueRandomValue()
    {
      return Faker.Name.FullName(GendersSelect.Value);
    }
  }
}