using ProjectIndustries.ProjectRaffles.Core.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates
{
  public interface IRaffleModuleExpandContextFactory
  {
    IRaffleModuleExpandContext Create(IRaffleModule module, IRaffleExecutionContext context);
  }
}