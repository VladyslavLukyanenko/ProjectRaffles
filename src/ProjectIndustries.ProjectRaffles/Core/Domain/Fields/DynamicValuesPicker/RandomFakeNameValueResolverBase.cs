using System.Collections.Generic;
using Bogus.DataSets;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public abstract class RandomFakeNameValueResolverBase : FakerRandomValueResolverBase
  {
    private static readonly KeyValuePair<string, object>[] Genders =
    {
      new KeyValuePair<string, object>("Male", Bogus.DataSets.Name.Gender.Male),
      new KeyValuePair<string, object>("Female", Bogus.DataSets.Name.Gender.Female),
    };


    protected readonly SelectField<Name.Gender> GendersSelect =
      new SelectField<Name.Gender>(displayName: "Gender", options: Genders)
      {
        Value = Bogus.DataSets.Name.Gender.Male
      };

    protected RandomFakeNameValueResolverBase(string name, string configWindowTitle)
      : base(name, configWindowTitle)
    {
    }

    protected override IEnumerable<Field> GetConfigFields()
    {
      yield return GendersSelect;
    }
  }
}