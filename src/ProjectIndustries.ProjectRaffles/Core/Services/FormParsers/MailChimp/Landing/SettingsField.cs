using System.Collections.Generic;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing
{
  public class SettingsField
  {
    public string Label { get; set; }
    public string Name { get; set; }

    [JsonProperty("merge_id")] public int? MergeId { get; set; }
    public string Type { get; set; }
    [JsonProperty("required")] public bool IsRequired { get; set; }
    public IList<MailChimpDropdownOption> Choices { get; set; } = new List<MailChimpDropdownOption>();
    [JsonProperty("dateformat")] public string DateFormat { get; set; }
  }
}