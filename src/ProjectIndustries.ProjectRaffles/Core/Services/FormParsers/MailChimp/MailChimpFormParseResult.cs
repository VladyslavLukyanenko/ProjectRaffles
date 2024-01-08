using System;
using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp
{
  public class MailChimpFormParseResult : FormParseResult
  {
    public MailChimpFormParseResult(string title, Uri sourceUrl, IEnumerable<Field> fields, string rawResponse,
      IFormSubmitHandler submitHandler)
      : base(title, sourceUrl, fields, rawResponse)
    {
      SubmitHandler = submitHandler;
    }


    public IFormSubmitHandler SubmitHandler { get; }
  }
}