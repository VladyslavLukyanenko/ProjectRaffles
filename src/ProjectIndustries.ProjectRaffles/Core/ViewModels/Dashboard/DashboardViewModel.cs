using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using NodaTime;
using ProjectIndustries.ProjectRaffles.WpfUI.MapBox.MapboxNetCore;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Dashboard
{
  public class DashboardViewModel : PageViewModelBase, IRoutableViewModel
  {
    public DashboardViewModel(IScreen hostScreen, IMessageBus messageBus, RecentEntriesViewModel recentEntries,
      PredefinedRafflesViewModel predefinedRaffles)
      : base("Dashboard", messageBus)
    {
      HostScreen = hostScreen;
      RecentEntries = recentEntries;
      PredefinedRaffles = predefinedRaffles;
      DisplayDate = DateTime.Now;
      this.WhenAnyValue(_ => _.DisplayDate)
        .Subscribe(d => { PredefinedRaffles.YearMonth = new YearMonth(d.Year, d.Month); });
      var offsetMillis = (int) TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalMilliseconds;
      var offset = Offset.FromMilliseconds(offsetMillis);
      predefinedRaffles.WhenAnyValue(_ => _.Specs)
        .Select(_ => _.Select(r =>
          r.ReleaseAt.WithOffset(offset).LocalDateTime.ToDateTimeUnspecified().Date).ToList())
        .ToPropertyEx(this, _ => _.HighlightedDates);

    }

    public IList<DateTime> HighlightedDates { [ObservableAsProperty] get; }
    [Reactive] public DateTime DisplayDate { get; set; }
    public string UrlPathSegment => nameof(DashboardViewModel);
    public IScreen HostScreen { get; }
    public RecentEntriesViewModel RecentEntries { get; }
    public PredefinedRafflesViewModel PredefinedRaffles { get; }
  }
}