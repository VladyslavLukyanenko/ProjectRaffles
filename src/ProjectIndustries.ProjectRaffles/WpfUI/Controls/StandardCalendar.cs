using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Controls
{
  public class StandardCalendar : Calendar
  {
    public static readonly DependencyProperty HighlightedDatesProperty =
      DependencyProperty.Register(nameof(HighlightedDates), typeof(IList<DateTime>), typeof(StandardCalendar));

    public IList<DateTime> HighlightedDates
    {
      get => (IList<DateTime>) GetValue(HighlightedDatesProperty);
      set => SetValue(HighlightedDatesProperty, value);
    }

    public static readonly DependencyProperty MonthNameProperty =
      DependencyProperty.Register(nameof(MonthName), typeof(string), typeof(StandardCalendar));

    public string MonthName
    {
      get => (string) GetValue(MonthNameProperty);
      set => SetValue(MonthNameProperty, value);
    }

    public static readonly DependencyProperty YearProperty =
      DependencyProperty.Register(nameof(Year), typeof(string), typeof(StandardCalendar));

    public string Year
    {
      get => (string) GetValue(YearProperty);
      set => SetValue(YearProperty, value);
    }

    protected override void OnDisplayDateChanged(CalendarDateChangedEventArgs e)
    {
      Year = DisplayDate.Year.ToString("D4");
      MonthName = DisplayDate.ToString("MMMM");
      base.OnDisplayDateChanged(e);
    }
  }
}