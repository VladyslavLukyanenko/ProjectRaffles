using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates.Expanders
{
  public class GuidVariableExpander : ITemplateVariableExpander
  {
    private static readonly string[] SupportedFormats =
    {
      "N", "B", "D", "P"
    };

    private const string FormatParam = "format";
    private const string DefaultFormat = "N";
    public string Name => "Guid";

    public string Expand(IDictionary<string, string> parameters, ITemplateExpandContext context)
    {
      string format = DefaultFormat;
      if (parameters.TryGetValue(FormatParam, out var incomeFormat))
      {
        incomeFormat = incomeFormat.ToUpperInvariant();
        if (SupportedFormats.Contains(incomeFormat))
        {
          format = incomeFormat;
        }
      }

      return Guid.NewGuid().ToString(format);
    }
  }
}