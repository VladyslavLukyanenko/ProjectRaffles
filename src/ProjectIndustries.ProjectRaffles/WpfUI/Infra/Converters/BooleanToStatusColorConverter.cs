using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Infra.Converters
{
  public class BooleanToStatusColorConverter : IValueConverter
  {
    public static readonly IValueConverter Instance = new BooleanToStatusColorConverter();
    public static readonly Brush PositiveColorBrush = new SolidColorBrush(Color.FromRgb(0, 255, 148));
    public static readonly Brush NegativeColorBrush = new SolidColorBrush(Color.FromRgb(255, 82, 96));


    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (bool)value ? PositiveColorBrush : NegativeColorBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
