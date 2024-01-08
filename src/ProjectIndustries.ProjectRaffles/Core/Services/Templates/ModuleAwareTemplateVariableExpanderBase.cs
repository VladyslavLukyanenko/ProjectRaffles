using System;
using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates
{
  public abstract class ModuleAwareTemplateVariableExpanderBase : IModuleAwareTemplateVariableExpander
  {
    string ITemplateVariableExpander.Expand(IDictionary<string, string> parameters, ITemplateExpandContext context)
    {
      if (context is not IRaffleModuleExpandContext)
      {
        throw new InvalidOperationException(
          $"Variable '{Name}' cannot be used in this context. It requires '{nameof(IRaffleModuleExpandContext)}'");
      }

      return Expand(parameters, (IRaffleModuleExpandContext) context);
    }

    public abstract string Name { get; }
    public abstract string Expand(IDictionary<string, string> parameters, IRaffleModuleExpandContext context);
  }
}