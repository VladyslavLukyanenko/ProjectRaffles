using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketLondonModule
{
  public class DoverStreetMarketLondonParsedRaffleFields
  {
    
    public DoverStreetMarketLondonParsedRaffleFields()
    {
    } 

    public DoverStreetMarketLondonParsedRaffleFields(string form, string viewkey, string viewparam, string fullnameField,
      string phoneNumberField, string emailField, string addressField, string sizeField, string baseUrl, string countryField, string postcodeField, string formstackSite, string mailingList, string colourField, string hiddenFields, string questionField, string shippingField)
    {
      Form = form;
      Viewkey = viewkey;
      Viewparam = viewparam;
      FullnameField = fullnameField;
      PhoneNumberField = phoneNumberField;
      EmailField = emailField;
      AddressField = addressField;
      SizeField = sizeField;
      BaseUrl = baseUrl;
      CountryField = countryField;
      PostCodeField = postcodeField;
      FormstackSite = formstackSite;
      MailingList = mailingList;
      ColourField = colourField;
      HiddenFields = hiddenFields;
      QuestionField = questionField;
      ShippingField = shippingField;
    }

    [JsonProperty(nameof(Form)), BsonField(nameof(Form))]
    public string Form { get; set; }

    [JsonProperty(nameof(Viewkey)), BsonField(nameof(Viewkey))]
    public string Viewkey { get; set; }

    [JsonProperty(nameof(Viewparam)), BsonField(nameof(Viewparam))]
    public string Viewparam { get; set; }

    [JsonProperty(nameof(FullnameField)), BsonField(nameof(FullnameField))]
    public string FullnameField { get; set; }

    [JsonProperty(nameof(PhoneNumberField)), BsonField(nameof(PhoneNumberField))]
    public string PhoneNumberField { get; set; }
    
    [JsonProperty(nameof(EmailField)), BsonField(nameof(EmailField))]
    public string EmailField { get; set; }

    [JsonProperty(nameof(AddressField)), BsonField(nameof(AddressField))]
    public string AddressField { get; set; }

    [JsonProperty(nameof(SizeField)), BsonField(nameof(SizeField))]
    public string SizeField { get; set; }
    
    [JsonProperty(nameof(BaseUrl)), BsonField(nameof(BaseUrl))]
    public string BaseUrl { get; set; }
    
    [JsonProperty(nameof(CountryField)), BsonField(nameof(CountryField))]
    public string CountryField { get; set; }
    
    [JsonProperty(nameof(PostCodeField)), BsonField(nameof(PostCodeField))]
    public string PostCodeField { get; set; }

    [JsonProperty(nameof(FormstackSite)), BsonField(nameof(FormstackSite))]
    public string FormstackSite { get; set; }
    
    [JsonProperty(nameof(MailingList)), BsonField(nameof(MailingList))]
    public string MailingList { get; set; }
    
    [JsonProperty(nameof(ColourField)), BsonField(nameof(ColourField))]
    public string ColourField { get; set; }
    
    [JsonProperty(nameof(HiddenFields)), BsonField(nameof(HiddenFields))]
    public string HiddenFields { get; set; }
    
    [JsonProperty(nameof(QuestionField)), BsonField(nameof(QuestionField))]
    public string QuestionField { get; set; }
    
    [JsonProperty(nameof(ShippingField)), BsonField(nameof(ShippingField))]
    public string ShippingField { get; set; }
  }
}