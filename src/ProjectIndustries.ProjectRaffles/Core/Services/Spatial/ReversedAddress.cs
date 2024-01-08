using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Spatial
{
  public class ReversedAddress
  {
    [JsonProperty("house_number")] public string HouseNumber { get; set; }
    [JsonProperty("road")] public string Road { get; set; }
    [JsonProperty("suburb")] public string SubUrb { get; set; }
    [JsonProperty("city")] public string City { get; set; }
    [JsonProperty("state")] public string State { get; set; }
    [JsonProperty("postcode")] public string PostCode { get; set; }
    [JsonProperty("country")] public string Country { get; set; }
    [JsonProperty("country_code")] public string CountryCode { get; set; }

    public bool IsHouseAddress()
    {
      return !string.IsNullOrEmpty(HouseNumber)
             && !string.IsNullOrEmpty(Road)
             && (!string.IsNullOrEmpty(City) || !string.IsNullOrEmpty(SubUrb))
             && !string.IsNullOrEmpty(State)
             && !string.IsNullOrEmpty(PostCode)
             && !PostCode.Contains("-")
             && !string.IsNullOrEmpty(Country)
             && !string.IsNullOrEmpty(CountryCode);
    }
  }
}