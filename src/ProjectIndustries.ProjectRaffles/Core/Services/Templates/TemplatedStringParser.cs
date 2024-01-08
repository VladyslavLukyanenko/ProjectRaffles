using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates
{
  public class TemplatedStringParser : ITemplatedStringParser
  {
    private const char VarToken = '%';
    private const char VarParamDelim = '|';
    private const char VarParamValueDelim = '=';

    public IEnumerable<TemplateToken> Parse(string input)
    {
      var rawValue = new StringBuilder();

      for (var index = 0; index < input.Length; index++)
      {
        var c = input[index];
        if (c != VarToken)
        {
          rawValue.Append(c);
        }
        else
        {
          if (rawValue.Length > 0)
          {
            yield return new TemplateLiteral(rawValue.ToString());
            rawValue.Clear();
          }

          yield return ParseVariable(input, ref index);
          rawValue.Clear();
        }
      }

      if (rawValue.Length != 0)
      {
        yield return new TemplateLiteral(rawValue.ToString());
      }
    }

    private TemplateToken ParseVariable(string input, ref int index)
    {
      var endVariableIndex = input.IndexOf(VarToken, index + 1) + 1;
      if (endVariableIndex == -1)
      {
        throw new TemplateParseException("Malformed string. Not closed variable at " + index);
      }

      var rawVariable = input.Substring(index, endVariableIndex - index).Trim(VarToken);
      var tokens = rawVariable.Split(VarParamDelim, StringSplitOptions.RemoveEmptyEntries);
      var varName = tokens[0];
      var parameters = new Dictionary<string, string>();
      foreach (var token in tokens.Skip(1))
      {
        var paramTokens = token.Split(VarParamValueDelim, StringSplitOptions.RemoveEmptyEntries);
        if (paramTokens.Length > 2)
        {
          throw new TemplateParseException(
            $"Malformed parameters for variable '{varName}'. Index range {index}-{endVariableIndex}");
        }

        parameters[paramTokens[0]] = paramTokens.Skip(1).LastOrDefault();
      }

      index = endVariableIndex - 1;
      return new TemplateVariable(varName, parameters);
    }
  }
}