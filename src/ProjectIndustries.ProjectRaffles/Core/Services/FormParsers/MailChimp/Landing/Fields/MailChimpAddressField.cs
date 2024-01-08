using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing.Fields
{
  public class MailChimpAddressField : MailChildFieldBase
  {
    private static readonly IDictionary<string, string> AddressParts = new Dictionary<string, string>
    {
      {"addr1", "Address"},
      {"addr2", "Address Line 2"},
      {"city", "City"},
      {"state", "State/Prov/Region"},
      {"zip", "Postal/Zip"},
    };

    public override IEnumerable<Field> ConvertToFields()
    {
      foreach (var addressPart in AddressParts)
      {
        yield return new DynamicValuesPickerField($"{Name}[{addressPart.Key}]", addressPart.Value, IsRequired,
          groups: Pickers.All)
        {
          SelectedResolver = MailChimpFieldValueResolverFactory.GetForAddressPart(addressPart.Key)
        };
      }

      yield return new OptionsField($"{Name}[country]", "Country", IsRequired, MailChildCountries.Countries);
    }
  }
}