using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Infra.Converters
{
  public class ToUpperCaseTextConverter : IValueConverter
  {
    public static readonly IValueConverter Instance = new ToUpperCaseTextConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((string) value)?.ToUpper(CultureInfo.CurrentUICulture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
