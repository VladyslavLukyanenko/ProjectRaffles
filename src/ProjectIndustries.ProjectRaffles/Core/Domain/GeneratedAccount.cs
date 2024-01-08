using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  public class GeneratedAccount : ViewModelBase
  {
    public GeneratedAccount(RaffleModuleDescriptor moduleDescriptor, AccountGroup accountGroup, Account account)
    {
      ModuleDescriptor = moduleDescriptor;
      AccountGroup = accountGroup;
      Account = account;
    }

    public RaffleModuleDescriptor ModuleDescriptor { get; }
    public AccountGroup AccountGroup { get; }
    public Account Account { get; }
  }
}