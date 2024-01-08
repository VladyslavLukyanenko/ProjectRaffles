using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Modules
{
  public interface IRaffleModulesProvider
  {
    IReadOnlyList<RaffleModuleDescriptor> SupportedModules { get; }
  }
}