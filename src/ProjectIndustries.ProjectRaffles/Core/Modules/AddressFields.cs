using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public class AddressFields : FieldsPresetBase
  {
    public Field<string> FirstName { get; set; } = new DynamicValuesPickerField("First Name")
    {
      SelectedResolver = Pickers.ProfileFields.FirstName
    };

    public Field<string> LastName { get; set; } = new DynamicValuesPickerField("Last Name")
    {
      SelectedResolver = Pickers.ProfileFields.LastName
    };

    public Field<string> FullName { get; set; } = new DynamicValuesPickerField("Full Name")
    {
      SelectedResolver = Pickers.ProfileFields.FullName
    };

    public Field<string> PhoneNumber { get; set; } = new DynamicValuesPickerField("Phone Number")
    {
      SelectedResolver = Pickers.ProfileShippingAddressFields.PhoneNumber
    };

    public Field<string> AddressLine1 { get; set; } = new DynamicValuesPickerField("Address Line 1")
    {
      SelectedResolver = Pickers.ProfileShippingAddressFields.AddressLine1
    };
    
    public Field<string> AddressLine2 { get; set; } = new DynamicValuesPickerField("Address Line 2")
    {
      SelectedResolver = Pickers.ProfileShippingAddressFields.AddressLine2
    };

    public Field<string> PostCode { get; set; } = new DynamicValuesPickerField("Post Code")
    {
      SelectedResolver = Pickers.ProfileShippingAddressFields.ZipCode
    };

    public Field<string> City { get; set; } = new DynamicValuesPickerField("City")
    {
      SelectedResolver = Pickers.ProfileShippingAddressFields.City
    };

    public Field<string> ProvinceId { get; set; } = new DynamicValuesPickerField("Province Code")
    {
      SelectedResolver = Pickers.ProfileShippingAddressFields.ProvinceCode
    };

    public Field<string> CountryId { get; set; } = new DynamicValuesPickerField("Country ID")
    {
      SelectedResolver = Pickers.ProfileShippingAddressFields.CountryId
    };
  }
}