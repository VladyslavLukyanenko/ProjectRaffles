using System;
using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms
{
  public class GoogleFormParseResult : FormParseResult
  {
    public GoogleFormParseResult(string title, Uri sourceUrl, IEnumerable<Field> fields, string rawResponse,
      string siteCaptchaKey)
      : base(title, sourceUrl, fields, rawResponse)
    {
      SiteCaptchaKey = siteCaptchaKey;
    }

    public string SiteCaptchaKey { get; }
    public bool RequiresSolveCaptcha => !string.IsNullOrEmpty(SiteCaptchaKey);
  }
}