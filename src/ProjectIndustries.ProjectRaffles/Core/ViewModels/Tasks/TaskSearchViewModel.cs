using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Tasks
{
  public class TaskSearchViewModel : ViewModelBase
  {
    [Reactive] public string SearchTerm { get; set; }
    
    
    public ReactiveCommand<Unit, Unit> CreateCommand { get; internal set; }
    public ReactiveCommand<Unit, Unit> StartAllCommand { get; internal set; }
    public ReactiveCommand<Unit, Unit> StopAllCommand { get; internal set; }
    public ReactiveCommand<Unit, Unit> DeleteAllCommand { get; internal set; }
  }
}