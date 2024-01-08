using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AmigoSkateshopModule
{
    public class AmigoSkateShopParsedForm
    {
        public AmigoSkateShopParsedForm()
        {
        }
        
        public AmigoSkateShopParsedForm(string title, string formId)
        {
            Title = title;
            FormId = formId;
        }
        [JsonProperty(nameof(Title)), BsonField(nameof(Title))]
        public string Title { get; set; }
        [JsonProperty(nameof(FormId)), BsonField(nameof(FormId))]
        public string FormId { get; set; }
    }
}