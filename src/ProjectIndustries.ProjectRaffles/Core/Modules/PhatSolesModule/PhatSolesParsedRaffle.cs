using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PhatSolesModule
{
    public class PhatSolesParsedRaffle
    {
        public PhatSolesParsedRaffle()
        {
        }

        public PhatSolesParsedRaffle(string wpfc7, string wpfc7version, string locale, string unitTag, string container, string tel, string menu)
        {
            Wpfc7 = wpfc7;
            Wpfc7Version = wpfc7version;
            Locale = locale;
            UnitTag = unitTag;
            Container = container;
            Tel = tel;
            Menu = menu;
        }

        [JsonProperty(nameof(Wpfc7)), BsonField(nameof(Wpfc7))]
        public string Wpfc7 { get; set; }

        [JsonProperty(nameof(Wpfc7Version)), BsonField(nameof(Wpfc7Version))]
        public string Wpfc7Version { get; set; }

        [JsonProperty(nameof(Locale)), BsonField(nameof(Locale))]
        public string Locale { get; set; }

        [JsonProperty(nameof(UnitTag)), BsonField(nameof(UnitTag))]
        public string UnitTag { get; set; }

        [JsonProperty(nameof(Container)), BsonField(nameof(Container))]
        public string Container { get; set; }
        
        [JsonProperty(nameof(Tel)), BsonField(nameof(Tel))]
        public string Tel { get; set; }
        
        [JsonProperty(nameof(Menu)), BsonField(nameof(Menu))]
        public string Menu { get; set; }
    }
}