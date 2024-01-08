using System.Collections.Generic;
using System.Globalization;
using Bogus;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates.Expanders
{
  public class UniqueIndexVariableExpander : FakerBasedRandomVariableExpanderBase
  {
    public UniqueIndexVariableExpander() : base("UniqueIndex")
    {
    }

    protected override string Expand(IDictionary<string, string> parameters, Faker faker)
    {
      return faker.UniqueIndex.ToString(CultureInfo.InvariantCulture);
    }
  }
}