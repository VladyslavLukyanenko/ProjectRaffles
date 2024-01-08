using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates.Expanders
{
  public class ProfileCityVariableExpander : ModuleAwareTemplateVariableExpanderBase
  {
    public override string Name => "ProfileCity";

    public override string Expand(IDictionary<string, string> parameters, IRaffleModuleExpandContext context)
    {
      return context.ExecutionContext.Profile.ShippingAddress.City;
    }
  }
}