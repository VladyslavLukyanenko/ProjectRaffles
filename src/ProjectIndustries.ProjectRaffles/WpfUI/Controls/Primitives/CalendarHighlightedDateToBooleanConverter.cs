using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Controls.Primitives
{
  public class CalendarHighlightedDateToBooleanConverter : IMultiValueConverter
  {
    public static readonly IMultiValueConverter Instance = new CalendarHighlightedDateToBooleanConverter();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      // Exit if values not set
      
      if (values[0] == null || values[1] == null)
      {
        return null;
      }

      var date = (DateTime) values[0];
      var calendar = (StandardCalendar) values[1];
      if (!calendar.HighlightedDates.Any())
      {
        return Visibility.Collapsed;
      }

      return calendar.HighlightedDates.Contains(date.Date) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}