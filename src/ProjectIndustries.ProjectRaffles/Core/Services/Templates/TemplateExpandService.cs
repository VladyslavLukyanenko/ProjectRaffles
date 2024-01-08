using System;
using System.Text;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates
{
  public class TemplateExpandService : ITemplateExpandService
  {
    private readonly ITemplatedStringParser _parser;

    public TemplateExpandService(ITemplatedStringParser parser)
    {
      _parser = parser;
    }

    public string Expand(string input, ITemplateExpandContext context)
    {
      if (string.IsNullOrWhiteSpace(input))
      {
        throw new ArgumentNullException(nameof(input));
      }

      var result = new StringBuilder();
      foreach (var token in _parser.Parse(input))
      {
        var str = token switch
        {
          TemplateLiteral l => l.Value,
          TemplateVariable v => ExpandVariable(v, context),
          _ => throw new TemplateParseException($"Token of type '{token.TokenType}' not supported.")
        };

        result.Append(str);
      }

      return result.ToString();
    }

    private string ExpandVariable(TemplateVariable variable, ITemplateExpandContext context)
    {
      var expander =
        context.GetExpander(variable.Name); /*_variableExpanders.FirstOrDefault(_ => _.Name == variable.Name);
      if (expander == null)
      {
        throw new TemplateParseException($"Can't find expander for variable '{variable.Name}'");
      }*/

      return expander.Expand(variable.Params, context);
    }
  }
}