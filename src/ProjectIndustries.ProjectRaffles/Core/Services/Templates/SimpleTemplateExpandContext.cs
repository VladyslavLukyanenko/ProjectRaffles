using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates
{
  public class SimpleTemplateExpandContext : ITemplateExpandContext
  {
    private readonly IEnumerable<ITemplateVariableExpander> _variableExpanders;

    public SimpleTemplateExpandContext(IEnumerable<ITemplateVariableExpander> variableExpanders)
    {
      _variableExpanders = variableExpanders;
    }

    public ITemplateVariableExpander GetExpander(string varName)
    {
      return _variableExpanders.FirstOrDefault(_ => _.Name == varName)
             ?? throw new ArgumentException($"Invalid variable '{varName}'", nameof(varName));
    }
  }
}