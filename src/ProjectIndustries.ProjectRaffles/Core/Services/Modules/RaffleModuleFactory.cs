using System;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Modules
{
  public class RaffleModuleFactory : IRaffleModuleFactory
  {
    public IRaffleModule Create(IRaffleModule prototype)
    {
      var module = Create(prototype.GetType());
      module.InitializeFromPrototype(prototype);

      return module;
    }

    public IRaffleModule Create(Type moduleType)
    {
      return (IRaffleModule) Locator.Current.GetService(moduleType);
    }
  }
}