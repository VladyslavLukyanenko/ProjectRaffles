using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public class MiscDynamicValuesGroup : IDynamicValuesGroup
  {
    private static readonly RandomEmailValueResolver EmailResolver = new RandomEmailValueResolver();

    public string Group { get; } = "Misc";

    public IReadOnlyList<IDynamicValueResolver> ValueResolvers => new List<IDynamicValueResolver>
    {
      EmailResolver,
      new RandomCustomListItemValueResolver(),
      new RandomFullNameValueResolver(),
      new RandomFirstNameValueResolver(),
      new RandomLastNameValueResolver(),
      new RandomPhoneNumberValueResolver(),
      new CustomAnswerValueResolver(),
      new RandomAddressResolver()
    };

    public IDynamicValueResolver Email => EmailResolver;

    // NOTICE: not supported preselection yet
    // public IDynamicValueResolver ListItem => ListItemResolver;
    // public IDynamicValueResolver FullName => FullNameResolver;
  }
}