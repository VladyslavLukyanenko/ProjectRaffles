using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing.Fields
{
  public class MailChimpTextField : MailChildFieldBase
  {
    public override IEnumerable<Field> ConvertToFields()
    {
      yield return new DynamicValuesPickerField(Name, Label, IsRequired, groups: Pickers.All)
      {
        SelectedResolver = MailChimpFieldValueResolverFactory.GetForAddressPart(Type)
                           ?? MailChimpFieldValueResolverFactory.GetForRegularField(Type)
      };
    }
  }
}