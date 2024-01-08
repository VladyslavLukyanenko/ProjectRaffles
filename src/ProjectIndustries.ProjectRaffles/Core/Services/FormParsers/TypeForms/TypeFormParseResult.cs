using System;
using System.Collections.Generic;
using System.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms
{
  public class TypeFormParseResult : FormParseResult
  {
    public TypeFormParseResult(string title, Uri sourceUrl, IEnumerable<TypeFormField> fields,
      string rawResponse)
      : base(title, sourceUrl, fields.SelectMany(_ => _.Fields), rawResponse)
    {
      TypeFormFields = fields.ToList();
    }

    public IReadOnlyList<TypeFormField> TypeFormFields { get; private set; }

    public TypeFormParseResult Clone() =>
      new TypeFormParseResult(Title, SourceUrl, TypeFormFields.Select(_ => _.Clone()), RawResponse);
  }
}