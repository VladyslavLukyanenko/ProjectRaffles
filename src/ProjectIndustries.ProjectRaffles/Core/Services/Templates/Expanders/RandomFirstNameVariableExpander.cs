using Bogus;
using Bogus.DataSets;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates.Expanders
{
  public class RandomFirstNameVariableExpander : RandomNameVariableExpanderBase
  {
    public RandomFirstNameVariableExpander() : base("RandomFirstName")
    {
    }

    protected override string GetName(Faker faker, Name.Gender? gender) => faker.Name.FirstName(gender);
  }
}