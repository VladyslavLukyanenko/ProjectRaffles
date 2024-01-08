using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates.Expanders
{
  public class ProfilePhoneNumberVariableExpander : ModuleAwareTemplateVariableExpanderBase
  {
    public override string Name => "ProfilePhoneNumber";

    public override string Expand(IDictionary<string, string> parameters, IRaffleModuleExpandContext context)
    {
      return context.ExecutionContext.Profile.ShippingAddress.PhoneNumber;
    }
  }
}