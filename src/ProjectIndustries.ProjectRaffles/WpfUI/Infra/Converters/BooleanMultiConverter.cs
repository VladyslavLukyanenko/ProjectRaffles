using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Infra.Converters
{
  public class BooleanMultiConverter : IMultiValueConverter
  {
    public static readonly BooleanMultiConverter Instance = new BooleanMultiConverter();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      return values.Cast<bool>().All(v => v);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}