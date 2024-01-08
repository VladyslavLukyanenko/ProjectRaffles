using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Infra.Converters
{
    public class DoubleToCornerRadiusConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double side)
            {
                return new CornerRadius(side / 2);
            }

            throw new ArgumentException("Value should be of Double type");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
