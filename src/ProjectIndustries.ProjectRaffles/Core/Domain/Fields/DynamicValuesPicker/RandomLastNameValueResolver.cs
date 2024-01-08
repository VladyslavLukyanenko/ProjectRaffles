namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public class RandomLastNameValueResolver : RandomFakeNameValueResolverBase
  {
    public RandomLastNameValueResolver()
      : base("Random Last Name", "Configure Last Name Picker")
    {
    }

    protected override string GetNonUniqueRandomValue()
    {
      return Faker.Name.LastName(GendersSelect.Value);
    }
  }
}