using System.Collections.Generic;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing.Fields
{
  public abstract class MailChildFieldBase
  {
    public string Label { get; set; }
    public string Name { get; set; }

    [JsonProperty("merge_id")] public int? MergeId { get; set; }
    public string Type { get; set; }
    [JsonProperty("required")] public bool IsRequired { get; set; }

    public abstract IEnumerable<Field> ConvertToFields();
  }
}