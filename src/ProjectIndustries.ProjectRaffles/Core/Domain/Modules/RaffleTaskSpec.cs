using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NodaTime;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Modules
{
  public class RaffleTaskSpec
  {
    public RaffleTaskSpec()
    {
    }

    [JsonConstructor]
    public RaffleTaskSpec(string id, string providerName, string productName, string productPicture, long releaseAt,
      IEnumerable<RaffleTaskField> additionalFields)
    {
      Id = id;
      ProviderName = providerName;
      ProductName = productName;
      ProductPicture = productPicture;
      ReleaseAt = Instant.FromUnixTimeSeconds(releaseAt);
      AdditionalFields = additionalFields.ToList();
    }

    public string Id { get; set; }
    public string ProviderName { get; set; }
    public string ProductName { get; set; }
    public string ProductPicture { get; set; }
    public Instant ReleaseAt { get; set; }
    public IList<RaffleTaskField> AdditionalFields { get; set; } = new List<RaffleTaskField>();
  }
}