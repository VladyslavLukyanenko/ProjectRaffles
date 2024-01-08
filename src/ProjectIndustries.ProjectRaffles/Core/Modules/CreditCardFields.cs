using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public class CreditCardFields : FieldsPresetBase
  {
    public Field<string> Number { get; set; } = new DynamicValuesPickerField("Credit Card Number")
    {
      SelectedResolver = Pickers.ProfileCreditCardFields.Number
    };

    public Field<string> Month { get; set; } = new DynamicValuesPickerField("Credit Card Exp. Month")
    {
      SelectedResolver = Pickers.ProfileCreditCardFields.ExpirationMonth
    };

    public Field<string> Year { get; set; } = new DynamicValuesPickerField("Credit Card Exp. Year")
    {
      SelectedResolver = Pickers.ProfileCreditCardFields.ExpirationYear
    };

    public Field<string> Cvv { get; set; } = new DynamicValuesPickerField("Credit Card CVV")
    {
      SelectedResolver = Pickers.ProfileCreditCardFields.SecurityCode
    };
  }
}