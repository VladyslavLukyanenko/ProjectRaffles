using System;
using ProjectIndustries.ProjectRaffles.Core.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Modules
{
  public interface IRaffleModuleFactory
  {
    IRaffleModule Create(IRaffleModule prototype);
    IRaffleModule Create(Type moduleType);
  }
}