using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Dashboard
{
  public class CalendarViewModel : ViewModelBase
  {
    const int RowsCount = 6;
    public CalendarViewModel()
    {
      this.WhenAnyValue(_ => _.SelectedMonth)
        .Subscribe(month =>
        {
          var daysCount = month.Calendar.GetDaysInMonth(month.Year, month.Month);
          var days = Enumerable.Range(1, daysCount).Select(dayNum => month.OnDayOfMonth(dayNum));
          Weeks = Enumerable.Range(0, 6).Select(idx => new CalendarWeekViewModel(idx, month, RowsCount)).ToList();
        });
    }

    [Reactive] public IList<CalendarWeekViewModel> Weeks { get; private set; }
    [Reactive] public CalendarDayViewModel SelectedDay { get; set; }
    [Reactive] public YearMonth SelectedMonth { get; set; }
  }
}
