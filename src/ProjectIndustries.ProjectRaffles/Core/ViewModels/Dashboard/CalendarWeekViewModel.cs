using System;
using NodaTime;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Dashboard
{
  public class CalendarWeekViewModel : ViewModelBase
  {
    private int _idx;
    private YearMonth _month;

    public CalendarWeekViewModel(int idx, YearMonth month, int rowsCount)
    {
      _idx = idx;
      _month = month;
      var maxRowsWillBeFilled = (int)Math.Ceiling(month.Calendar.GetDaysInMonth(month.Year, month.Month) / 7d);
      var firstDay = month.OnDayOfMonth(1);
      var weekDay = firstDay.DayOfWeek;
      

    }

    [Reactive] public CalendarDayViewModel Sunday { get; set; }
    [Reactive] public CalendarDayViewModel Monday { get; set; }
    [Reactive] public CalendarDayViewModel Tuesday { get; set; }
    [Reactive] public CalendarDayViewModel Wednesday { get; set; }
    [Reactive] public CalendarDayViewModel Thursday { get; set; }
    [Reactive] public CalendarDayViewModel Friday { get; set; }
    [Reactive] public CalendarDayViewModel Saturday { get; set; }
  }
}