using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing.Fields
{
  public class MailChildDateFormatField : MailChildFieldBase
  {
    private static readonly Dictionary<string, string> FieldNames = new Dictionary<string, string>
    {
      {"yyyy", "[year]"},
      {"mm", "[month]"},
      {"dd", "[day]"}
    };

    [JsonProperty("dateformat")] public string DateFormat { get; set; }

    public override IEnumerable<Field> ConvertToFields()
    {
      return DateFormat.Split('/', StringSplitOptions.RemoveEmptyEntries)
        .Select(part => new DynamicValuesPickerField($"{Name}{FieldNames[part.Trim().ToLowerInvariant()]}",
          $"{Label} - {part}", IsRequired, groups: Pickers.All))
        .ToArray();
    }
  }
}