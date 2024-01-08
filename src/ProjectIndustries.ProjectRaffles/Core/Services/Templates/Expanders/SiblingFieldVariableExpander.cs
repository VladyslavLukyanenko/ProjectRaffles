using System.Collections.Generic;
using System.Linq;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates.Expanders
{
  public class SiblingFieldVariableExpander : ModuleAwareTemplateVariableExpanderBase
  {
    public override string Name => "SiblingField";

    public override string Expand(IDictionary<string, string> parameters, IRaffleModuleExpandContext context)
    {
      var name = parameters.Keys.First().ToLowerInvariant();
      return context.Fields.FirstOrDefault(_ =>
               _.SystemName.ToLowerInvariant() == name || _.DisplayName.ToLowerInvariant() == name)
             ?? throw new TemplateParseException($"Can't find field with name '{name}'");
    }
  }
}