using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates.Expanders
{
  public class ProfileFirstNameVariableExpander : ModuleAwareTemplateVariableExpanderBase
  {
    public override string Name => "ProfileFirstName";

    public override string Expand(IDictionary<string, string> parameters, IRaffleModuleExpandContext context)
    {
      return context.ExecutionContext.Profile.ShippingAddress.FirstName;
    }
  }
}