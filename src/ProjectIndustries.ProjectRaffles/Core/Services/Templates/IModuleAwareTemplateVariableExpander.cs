using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates
{
  public interface IModuleAwareTemplateVariableExpander : ITemplateVariableExpander
  {
    string Expand(IDictionary<string, string> parameters, IRaffleModuleExpandContext context);
  }
}