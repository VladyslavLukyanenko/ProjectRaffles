using System;
using System.Collections.Generic;
using System.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers
{
  public class FormParseResult
  {
    public FormParseResult(string title, Uri sourceUrl, IEnumerable<Field> fields, string rawResponse,
      params string[] warnings)
    {
      SourceUrl = sourceUrl;
      Fields = fields.ToArray();
      RawResponse = rawResponse;
      Title = title;
      Warnings = warnings;
    }

    public string Title { get; }
    public Uri SourceUrl { get; }
    public IReadOnlyList<Field> Fields { get; }
    public string RawResponse { get; }
    public IEnumerable<string> Warnings { get; }
  }
}