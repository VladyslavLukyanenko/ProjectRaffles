using System.Collections.Generic;
using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.MahaAccountGenerator
{
    public class MahaAccountGeneratorParsed
    {
        public MahaAccountGeneratorParsed()
        {
        }

        public MahaAccountGeneratorParsed(Dictionary<string, string> countryDictionary,
            Dictionary<string, bool> hasRegions, Dictionary<string, string> countryType)
        {
            CountryDictionary = countryDictionary;
            HasRegions = hasRegions;
            CountryType = countryType;
        }

        [JsonProperty(nameof(CountryDictionary)), BsonField(nameof(CountryDictionary))]
        public Dictionary<string, string> CountryDictionary { get; set; }

        [JsonProperty(nameof(HasRegions)), BsonField(nameof(HasRegions))]
        public Dictionary<string, bool> HasRegions { get; set; }

        [JsonProperty(nameof(CountryType)), BsonField(nameof(CountryType))]
        public Dictionary<string, string> CountryType { get; set; }
    }
}