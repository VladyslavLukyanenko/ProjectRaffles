using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Infra.Converters
{
  public class InverseBooleanConverter : IValueConverter
  {
    public static readonly IValueConverter Instance = new InverseBooleanConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is bool b && !b;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Convert(value, targetType, parameter, culture);
    }
  }
}