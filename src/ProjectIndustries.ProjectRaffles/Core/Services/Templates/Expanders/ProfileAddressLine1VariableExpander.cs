using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates.Expanders
{
  public class ProfileAddressLine1VariableExpander : ModuleAwareTemplateVariableExpanderBase
  {
    public override string Name => "ProfileAddressLine1";

    public override string Expand(IDictionary<string, string> parameters, IRaffleModuleExpandContext context)
    {
      return context.ExecutionContext.Profile.ShippingAddress.AddressLine1;
    }
  }
}