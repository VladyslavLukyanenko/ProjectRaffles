using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates.Expanders
{
  public class ProfileCountryIdVariableExpander : ModuleAwareTemplateVariableExpanderBase
  {
    public override string Name => "ProfileCountryId";

    public override string Expand(IDictionary<string, string> parameters, IRaffleModuleExpandContext context)
    {
      return context.ExecutionContext.Profile.ShippingAddress.CountryId;
    }
  }
}