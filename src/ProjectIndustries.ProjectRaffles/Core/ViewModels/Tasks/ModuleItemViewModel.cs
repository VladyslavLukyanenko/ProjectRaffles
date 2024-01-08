using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Tasks
{
  public class ModuleItemViewModel
  {
    public ModuleItemViewModel(RaffleModuleDescriptor moduleDescriptor, bool isActive)
    {
      ModuleDescriptor = moduleDescriptor;
      IsActive = isActive;
    }

    public RaffleModuleDescriptor ModuleDescriptor { get; }
    public bool IsActive { get; }
  }
}