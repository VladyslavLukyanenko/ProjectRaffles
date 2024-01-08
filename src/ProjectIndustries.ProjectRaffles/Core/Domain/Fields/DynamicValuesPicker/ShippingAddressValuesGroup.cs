using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public class ShippingAddressValuesGroup : IDynamicValuesGroup
  {
    private static readonly IDynamicValueResolver AddressLine1Resolver = DynamicValuesGroupFactory.Create(
      ctx => ctx.Profile,
      _ => _.ShippingAddress.AddressLine1);

    private static readonly IDynamicValueResolver AddressLine2Resolver = DynamicValuesGroupFactory.Create(
      ctx => ctx.Profile,
      _ => _.ShippingAddress.AddressLine2);

    private static readonly IDynamicValueResolver CountryIdResolver = DynamicValuesGroupFactory.Create(
      ctx => ctx.Profile,
      _ => _.ShippingAddress.CountryId);

    private static readonly IDynamicValueResolver ProvinceCodeResolver = DynamicValuesGroupFactory.Create(
      ctx => ctx.Profile,
      _ => _.ShippingAddress.ProvinceCode);

    private static readonly IDynamicValueResolver PhoneNumberResolver = DynamicValuesGroupFactory.Create(
      ctx => ctx.Profile,
      _ => _.ShippingAddress.PhoneNumber);

    private static readonly IDynamicValueResolver ZipCodeResolver = DynamicValuesGroupFactory.Create(
      ctx => ctx.Profile,
      _ => _.ShippingAddress.ZipCode);

    private static readonly IDynamicValueResolver CityResolver = DynamicValuesGroupFactory.Create(
      ctx => ctx.Profile,
      _ => _.ShippingAddress.City);

    public ShippingAddressValuesGroup()
    {
      ValueResolvers = new List<IDynamicValueResolver>
        {
          AddressLine1Resolver,
          AddressLine2Resolver,
          CountryIdResolver,
          ProvinceCodeResolver,
          PhoneNumberResolver,
          ZipCodeResolver,
          CityResolver
        }
        .AsReadOnly();
    }

    public IDynamicValueResolver AddressLine1 => AddressLine1Resolver;
    public IDynamicValueResolver AddressLine2 => AddressLine2Resolver;
    public IDynamicValueResolver CountryId => CountryIdResolver;
    public IDynamicValueResolver ProvinceCode => ProvinceCodeResolver;
    public IDynamicValueResolver PhoneNumber => PhoneNumberResolver;
    public IDynamicValueResolver ZipCode => ZipCodeResolver;
    public IDynamicValueResolver City => CityResolver;


    public string Group => "Shipping Address";
    public IReadOnlyList<IDynamicValueResolver> ValueResolvers { get; }
  }
}