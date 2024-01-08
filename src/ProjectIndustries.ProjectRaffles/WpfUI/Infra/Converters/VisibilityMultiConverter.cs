using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Infra.Converters
{
  public class VisibilityMultiConverter
      : IMultiValueConverter
  {
    public static readonly IMultiValueConverter Instance = new VisibilityMultiConverter();
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      return values.OfType<Visibility>().All(_ => _ == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
