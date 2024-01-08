using ProjectIndustries.ProjectRaffles.Core.Domain;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Tasks
{
  public class TaskProfileViewModel : ViewModelBase
  {
    public TaskProfileViewModel(Profile profile)
    {
      Profile = profile;
    }

    public Profile Profile { get; }
    [Reactive] public bool IsEnabled { get; set; }
  }
}