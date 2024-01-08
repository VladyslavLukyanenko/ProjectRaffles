using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Modules
{
  public class RaffleModulesProvider : IRaffleModulesProvider
  {
    public static readonly IReadOnlyList<RaffleModuleDescriptor> Modules = typeof(IRaffleModule).Assembly
      .GetExportedTypes()
      .Where(t =>
        typeof(IRaffleModule).IsAssignableFrom(t)
        && !t.IsAbstract
        && t.GetCustomAttribute<DisabledRaffleModuleAttribute>() == null)
      .Select(t =>
      {
        var nameAttr = t.GetCustomAttribute<RaffleModuleNameAttribute>();
        var captchaAttr = t.GetCustomAttribute<RaffleRequiresCaptchaAttribute>();
        var paymentMethodAttr = t.GetCustomAttribute<RafflePaymentMethodAttribute>();
        var extractorAttr = t.GetCustomAttribute<MailRaffleStatusExtractorAttribute>();
        var confirmHandlerAttr = t.GetCustomAttribute<MailRaffleConfirmationHandlerAttribute>();
        var generatorAttr = t.GetCustomAttribute<RaffleAccountGeneratorAttribute>();
        var typeAttr = t.GetCustomAttribute<RaffleModuleTypeAttribute>();

        var requiresCreditCard = paymentMethodAttr?.PaymentMethod == PaymentMethod.CreditCard;
        var requiresCaptcha = captchaAttr?.CaptchaRequirement == CaptchaRequirement.Required;
        var noProfileRequires = t.GetCustomAttribute<NoProfileRequiresAttribute>() != null;

        return new RaffleModuleDescriptor(nameAttr!.Name, t, requiresCaptcha, requiresCreditCard,
          extractorAttr?.ExtractorType, confirmHandlerAttr?.ExtractorType, generatorAttr?.GeneratorType,
          typeAttr!.Type, noProfileRequires);
      })
      .OrderBy(_ => _.Name)
      .ToList()
      .AsReadOnly();

    public IReadOnlyList<RaffleModuleDescriptor> SupportedModules => Modules;
  }
}