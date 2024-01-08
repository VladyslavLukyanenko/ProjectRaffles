using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public class ProfileValuesGroup : IDynamicValuesGroup
  {
    private static readonly IDynamicValueResolver FirstNameResolver = DynamicValuesGroupFactory.Create(
      ctx => ctx.Profile,
      _ => _.ShippingAddress.FirstName);

    private static readonly IDynamicValueResolver LastNameResolver = DynamicValuesGroupFactory.Create(
      ctx => ctx.Profile,
      _ => _.ShippingAddress.LastName);

    private static readonly IDynamicValueResolver FullNameResolver = DynamicValuesGroupFactory.Create(
      ctx => ctx.Profile,
      _ => _.ShippingAddress.FullName);
    
    public ProfileValuesGroup()
    {
      ValueResolvers = new List<IDynamicValueResolver>
        {
          FirstNameResolver,
          LastNameResolver,
          FullNameResolver
        }
        .AsReadOnly();
    }

    public IDynamicValueResolver FirstName => FirstNameResolver;
    public IDynamicValueResolver LastName => LastNameResolver;
    public IDynamicValueResolver FullName => FullNameResolver;

    public string Group => "Profile";
    public IReadOnlyList<IDynamicValueResolver> ValueResolvers { get; }
  }
}