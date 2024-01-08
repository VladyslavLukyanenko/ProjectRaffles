using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public class CreditCardValuesGroup : IDynamicValuesGroup
  {
    private static readonly IDynamicValueResolver CreditCardNumberResolver = DynamicValuesGroupFactory.Create(
      ctx => ctx.Profile,
      _ => _.CreditCard.Number);

    private static readonly IDynamicValueResolver CreditCardExpirationMonthResolver = DynamicValuesGroupFactory.Create(
      ctx => ctx.Profile,
      _ => _.CreditCard.ExpirationMonth);

    private static readonly IDynamicValueResolver CreditCardExpirationYearResolver = DynamicValuesGroupFactory.Create(
      ctx => ctx.Profile,
      _ => _.CreditCard.ExpirationYear);

    private static readonly IDynamicValueResolver CreditCardSecurityCodeResolver = DynamicValuesGroupFactory.Create(
      ctx => ctx.Profile,
      _ => _.CreditCard.Cvv);

    public CreditCardValuesGroup()
    {
      ValueResolvers = new List<IDynamicValueResolver>
        {
          CreditCardNumberResolver,
          CreditCardExpirationMonthResolver,
          CreditCardExpirationYearResolver,
          CreditCardSecurityCodeResolver
        }
        .AsReadOnly();
    }

    public IDynamicValueResolver Number => CreditCardNumberResolver;
    public IDynamicValueResolver ExpirationMonth => CreditCardExpirationMonthResolver;
    public IDynamicValueResolver ExpirationYear => CreditCardExpirationYearResolver;
    public IDynamicValueResolver SecurityCode => CreditCardSecurityCodeResolver;

    public string Group => "Credit Card";
    public IReadOnlyList<IDynamicValueResolver> ValueResolvers { get; }
  }
}