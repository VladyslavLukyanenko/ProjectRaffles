using System.Collections.Generic;
using Bogus;
using Bogus.DataSets;
using Unidecode.NET;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates.Expanders
{
  public abstract class RandomNameVariableExpanderBase : FakerBasedRandomVariableExpanderBase
  {
    private const string GenderParam = "gender";
    private const string TransliterateParam = "transliterate";
    private const string Male = "M";
    private const string Female = "F";

    protected RandomNameVariableExpanderBase(string name) : base(name)
    {
    }

    protected override string Expand(IDictionary<string, string> parameters, Faker faker)
    {
      Name.Gender? gender = null;
      if (parameters.TryGetValue(GenderParam, out var genderStr))
      {
        gender = genderStr switch
        {
          Male => Bogus.DataSets.Name.Gender.Male,
          Female => Bogus.DataSets.Name.Gender.Female,
          _ => null
        };
      }

      var name = GetName(faker, gender);
      if (parameters.ContainsKey(TransliterateParam))
      {
        name = name.Unidecode();
      }

      return name;
    }

    protected abstract string GetName(Faker faker, Name.Gender? gender);
  }
}