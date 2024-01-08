using Bogus;
using Bogus.DataSets;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates.Expanders
{
  public class RandomLastNameVariableExpander : RandomNameVariableExpanderBase
  {
    public RandomLastNameVariableExpander() : base("RandomLastName")
    {
    }

    protected override string GetName(Faker faker, Name.Gender? gender) => faker.Name.LastName(gender);
  }
}