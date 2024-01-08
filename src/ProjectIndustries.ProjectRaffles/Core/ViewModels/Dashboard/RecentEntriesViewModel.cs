using System.Collections.Generic;
using System.Reactive.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Dashboard
{
  public class RecentEntriesViewModel : ViewModelBase
  {
    public RecentEntriesViewModel(IStatsService statsService)
    {
      var stats = statsService.Stats
        .ObserveOn(RxApp.MainThreadScheduler);
      
      stats.ToPropertyEx(this, _ => _.Entries);
      stats.Select(e => e.Count == 0).ToPropertyEx(this, _ => _.NoEntriesYet);
    }

    public IList<SubmissionStatsEntry> Entries { [ObservableAsProperty] get; }
    
    public bool NoEntriesYet { [ObservableAsProperty] get; }
  }
}