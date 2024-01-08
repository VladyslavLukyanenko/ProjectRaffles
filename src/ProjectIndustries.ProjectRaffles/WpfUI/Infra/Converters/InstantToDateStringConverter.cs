using System;
using System.Globalization;
using System.Windows.Data;
using NodaTime;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Infra.Converters
{
  public class InstantToDateStringConverter : IValueConverter
  {
    public static readonly IValueConverter Instance = new InstantToDateStringConverter();
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Instant i)
      {
        var offsetMillis = (int) TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalMilliseconds;
        var offset = Offset.FromMilliseconds(offsetMillis);
        return i.WithOffset(offset)
          .LocalDateTime
          .Date
          .ToString("MMMM dd", CultureInfo.CurrentCulture);
      }
      
      throw new ArgumentException("Invalid type provided. Only NodaTime.Instant supported");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}