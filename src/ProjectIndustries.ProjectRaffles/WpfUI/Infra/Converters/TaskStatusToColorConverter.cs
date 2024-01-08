using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Infra.Converters
{
  public class TaskStatusToColorConverter
    : IValueConverter
  {
    public static readonly TaskStatusToColorConverter Instance = new TaskStatusToColorConverter();

    private static readonly Brush FallbackBrush = new SolidColorBrush(Colors.Red);

    private static readonly IDictionary<RaffleStatusKind, Brush> Brushes = new Dictionary<RaffleStatusKind, Brush>
    {
      {RaffleStatusKind.Created, new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88))},
      {RaffleStatusKind.Preparation, new SolidColorBrush(Color.FromRgb(0x00, 0xaF, 0xc4))},
      {RaffleStatusKind.Ready, new SolidColorBrush(Color.FromRgb(0xFC, 0xAD, 0xB3))},
      {RaffleStatusKind.InProgress, new SolidColorBrush(Color.FromRgb(0xFF, 0x5C, 0x00))},
      {RaffleStatusKind.Succeeded, new SolidColorBrush(Color.FromRgb(0x00, 0xFF, 0x94))},
      {RaffleStatusKind.Failed, new SolidColorBrush(Color.FromRgb(0xDE, 0x1C, 0x1C))},
      {RaffleStatusKind.Cancelled, new SolidColorBrush(Color.FromRgb(0xDE, 0xDE, 0xDE))},
    };

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is RaffleStatusKind status)
      {
        return Brushes[status];
      }

      return FallbackBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}