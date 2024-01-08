using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Infra.Converters
{
  public class RawPathToImageSourceConverter
    : IValueConverter
  {
    private const string FallbackImage = "/Assets/image-not-found.jpg";
    private static readonly string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
    public static readonly RawPathToImageSourceConverter Instance = new RawPathToImageSourceConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if (value != null && value is string path)
        {
          if (!path.StartsWith("http") && !path.StartsWith("pack"))
          {
            path = $"pack://application:,,,/{AssemblyName};component{path}";
          }

          var uri = new Uri(path, UriKind.RelativeOrAbsolute);

          return new BitmapImage(uri);
        }
      }
      catch
      {
        // noop
      }

      return Convert(FallbackImage, targetType, parameter, culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}