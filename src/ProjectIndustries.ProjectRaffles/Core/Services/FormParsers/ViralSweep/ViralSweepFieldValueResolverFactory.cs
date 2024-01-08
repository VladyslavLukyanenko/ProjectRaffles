using System.Collections.Generic;
using System.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep
{
  public static class ViralSweepFieldValueResolverFactory
  {
    private static readonly IDictionary<string, IDynamicValueResolver> ResolversMapping =
      new Dictionary<string, IDynamicValueResolver>
      {
        {"first_name", Pickers.ProfileFields.FirstName},
        {"last_name", Pickers.ProfileFields.LastName},
        {"email", Pickers.Misc.Email},
        {"address", Pickers.ProfileShippingAddressFields.AddressLine1},
        {"address2", Pickers.ProfileShippingAddressFields.AddressLine2},
        {"city", Pickers.ProfileShippingAddressFields.City},
        {"zip", Pickers.ProfileShippingAddressFields.ZipCode},
        {"phone", Pickers.ProfileShippingAddressFields.PhoneNumber}
      };

    public static IDynamicValueResolver Resolve(IEnumerable<string> classes, string name) =>
      ResolversMapping.TryGetValue(name, out var resolver)
        ? resolver
        : TryCreditCardResolve(name)
          ?? ResolversMapping.Where(p => classes.Contains(p.Key))
            .Select(_ => _.Value)
            .FirstOrDefault();

    private static IDynamicValueResolver TryCreditCardResolve(string name)
    {
      if (name.EndsWith("card_cvc"))
      {
        return Pickers.ProfileCreditCardFields.SecurityCode;
      }

      if (name.EndsWith("card_number"))
      {
        return Pickers.ProfileCreditCardFields.Number;
      }

      return null;
    }
  }
}