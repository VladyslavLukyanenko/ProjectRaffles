using System;
using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep
{
  public class ViralSweepParseResult : FormParseResult
  {
    public ViralSweepParseResult(string title, Uri sourceUrl, IEnumerable<Field> fields, string rawResponse,
      string reCaptchaSiteKey, IFormSubmitHandler submitHandler, string formUrl, string invalidGeoWarning)
      : base(title, sourceUrl, fields, rawResponse,
        string.IsNullOrEmpty(invalidGeoWarning) ? Array.Empty<string>() : new[] {invalidGeoWarning})
    {
      ReCaptchaSiteKey = reCaptchaSiteKey;
      SubmitHandler = submitHandler;
      FormUrl = formUrl;
    }

    public string ReCaptchaSiteKey { get; }
    public string FormUrl { get; }
    public IFormSubmitHandler SubmitHandler { get; }
  }
}