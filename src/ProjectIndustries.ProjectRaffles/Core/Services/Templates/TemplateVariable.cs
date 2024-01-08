using System.Collections.Generic;
using System.Linq;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates
{
  public class TemplateVariable : TemplateToken
  {
    public TemplateVariable(string name, IDictionary<string, string> @params)
    {
      Name = name;
      Params = @params;
    }

    public string Name { get; private set; }
    public IDictionary<string, string> Params { get; private set; }
    public override TemplateTokenType TokenType => TemplateTokenType.Variable;

    public override string ToString()
    {
      var @params = string.Join(", ", Params.Select(p => $"'{p.Key}'='{p.Value}'"));
      return $"'{Name}' {(string.IsNullOrEmpty(@params) ? "<No params>" : @params)}";
    }
  }
}