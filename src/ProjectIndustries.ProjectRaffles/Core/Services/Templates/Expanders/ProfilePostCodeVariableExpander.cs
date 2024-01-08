using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates.Expanders
{
  public class ProfilePostCodeVariableExpander : ModuleAwareTemplateVariableExpanderBase
  {
    public override string Name => "ProfilePostCode";

    public override string Expand(IDictionary<string, string> parameters, IRaffleModuleExpandContext context)
    {
      return context.ExecutionContext.Profile.ShippingAddress.City;
    }
  }
}