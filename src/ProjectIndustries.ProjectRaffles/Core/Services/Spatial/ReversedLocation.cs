using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Spatial
{
  public class ReversedLocation
  {
    [JsonProperty("display_name")] public string DisplayName { get; set; }
    [JsonProperty("address")] public ReversedAddress Address { get; set; }
  }
}