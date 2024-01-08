using System.Reflection;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Profiles
{
  [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
  public class CsvProfileData
  {
    public string BillingAddressFirstName { get; set; }
    public string BillingAddressLastName { get; set; }
    public string BillingAddressAddressLine1 { get; set; }
    public string BillingAddressAddressLine2 { get; set; }
    public string BillingAddressCity { get; set; }
    public string BillingAddressZipCode { get; set; }
    public string BillingAddressCountryId { get; set; }
    public string BillingAddressPhoneNumber { get; set; }
    public string BillingAddressProvinceCode { get; set; }
      
    public string ShippingAddressFirstName { get; set; }
    public string ShippingAddressLastName { get; set; }
    public string ShippingAddressAddressLine1 { get; set; }
    public string ShippingAddressAddressLine2 { get; set; }
    public string ShippingAddressCity { get; set; }
    public string ShippingAddressZipCode { get; set; }
    public string ShippingAddressCountryId { get; set; }
    public string ShippingAddressPhoneNumber { get; set; }
    public string ShippingAddressProvinceCode { get; set; }
      
    public string CreditCardCvv { get; set; }
    public int CreditCardExpirationMonth { get; set; }
    public int CreditCardExpirationYear { get; set; }
    public string CreditCardNumber { get; set; }
    public bool IsShippingSameAsBilling { get; set; }
    public string ProfileName { get; set; }
  }
}