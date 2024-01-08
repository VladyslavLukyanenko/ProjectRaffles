using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates.Expanders
{
  public class ProfileLastNameVariableExpander : ModuleAwareTemplateVariableExpanderBase
  {
    public override string Name => "ProfileLastName";

    public override string Expand(IDictionary<string, string> parameters, IRaffleModuleExpandContext context)
    {
      return context.ExecutionContext.Profile.ShippingAddress.LastName;
    }
  }
}