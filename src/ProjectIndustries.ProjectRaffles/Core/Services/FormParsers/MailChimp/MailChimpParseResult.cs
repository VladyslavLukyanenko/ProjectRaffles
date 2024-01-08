using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp
{
  public class MailChimpParseResult
  {
    public MailChimpParseResult(IList<Field> fields, IFormSubmitHandler submitHandler)
    {
      Fields = fields;
      SubmitHandler = submitHandler;
    }

    public IList<Field> Fields { get; }
    public IFormSubmitHandler SubmitHandler { get; }
  }
}