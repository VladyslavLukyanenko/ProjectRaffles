using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketNewYorkModule
{
  public class DoverStreetMarketNewYorkParsedRaffleFields
  {
    
    public DoverStreetMarketNewYorkParsedRaffleFields()
    {
    } 

    public DoverStreetMarketNewYorkParsedRaffleFields(string form, string viewkey, string viewparam, string fullnameField,
      string phoneNumberField, string emailField, string addressField, string sizeField, string baseUrl, string stateField, string zipCodeField, string formstackSite, string mailingList, string colourField, string hiddenField, string questionField)
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
      StateField = stateField;
      ZipCodeField = zipCodeField;
      FormstackSite = formstackSite;
      MailingList = mailingList;
      ColourField = colourField;
      HiddenFields = hiddenField;
      QuestionField = questionField;
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
    
    [JsonProperty(nameof(StateField)), BsonField(nameof(StateField))]
    public string StateField { get; set; }
    
    [JsonProperty(nameof(ZipCodeField)), BsonField(nameof(ZipCodeField))]
    public string ZipCodeField { get; set; }

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
  }
}