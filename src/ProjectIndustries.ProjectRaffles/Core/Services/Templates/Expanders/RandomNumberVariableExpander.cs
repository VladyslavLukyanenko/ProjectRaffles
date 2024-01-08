using System;
using System.Collections.Generic;
using System.Globalization;
using Bogus;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates.Expanders
{
  public class RandomNumberVariableExpander : FakerBasedRandomVariableExpanderBase
  {
    private const string DecimalsParam = "digits";

    public RandomNumberVariableExpander() : base("RandomNumber")
    {
    }

    protected override string Expand(IDictionary<string, string> parameters, Faker faker)
    {
      int min = int.MinValue;
      int max = int.MaxValue;
      if (parameters.TryGetValue(DecimalsParam, out var digits))
      {
        var tokens = digits.Split('-', StringSplitOptions.RemoveEmptyEntries);
        var decimals = int.Parse(tokens[0]);
        min = (int) Math.Pow(10, decimals);
        int maxDecimals = decimals;
        if (tokens.Length == 2)
        {
          maxDecimals = int.Parse(tokens[1]);
        }

        max = (int) Math.Pow(10, maxDecimals + 1) - 1;
      }

      return faker.Random.Int(min, max).ToString(CultureInfo.InvariantCulture);
    }
  }
}