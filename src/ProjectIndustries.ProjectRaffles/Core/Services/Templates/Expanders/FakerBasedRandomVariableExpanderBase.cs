using System.Collections.Generic;
using Bogus;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates.Expanders
{
  public abstract class FakerBasedRandomVariableExpanderBase : ITemplateVariableExpander
  {
    private const string LocaleParam = "locale";
    private const string FallbackLocale = "en";

    protected FakerBasedRandomVariableExpanderBase(string name)
    {
      Name = name;
    }

    public string Name { get; }

    public string Expand(IDictionary<string, string> parameters, ITemplateExpandContext context)
    {
      if (!parameters.TryGetValue(LocaleParam, out var locale))
      {
        locale = FallbackLocale;
      }

      var faker = new Faker(locale);

      return Expand(parameters, faker);
    }

    protected abstract string Expand(IDictionary<string, string> parameters, Faker faker);
  }
}