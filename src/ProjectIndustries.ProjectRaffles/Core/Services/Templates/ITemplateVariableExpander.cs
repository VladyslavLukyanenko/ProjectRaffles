using System;
using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates
{
  public interface ITemplateVariableExpander
  {
    string Name { get; }
    string Expand(IDictionary<string, string> parameters, ITemplateExpandContext context);
  }
}