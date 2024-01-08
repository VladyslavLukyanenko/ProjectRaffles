using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TsumModule
{
  public class TsumProductTags
  {
    public TsumProductTags()
    {
    }

    public TsumProductTags(int landingRelation, int productRelation)
    {
      LandingRelation = landingRelation;
      ProductRelation = productRelation;
    }

    [JsonProperty(nameof(LandingRelation)), BsonField(nameof(LandingRelation))]
    public int LandingRelation { get; set; }

    [JsonProperty(nameof(ProductRelation)), BsonField(nameof(ProductRelation))]
    public int ProductRelation { get; set; }
  }
}