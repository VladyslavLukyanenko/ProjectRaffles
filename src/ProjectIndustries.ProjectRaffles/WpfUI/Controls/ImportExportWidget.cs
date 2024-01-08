using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Controls
{
  public class ImportExportWidget : Control
  {
    public static readonly DependencyProperty ImportAllCommandProperty = DependencyProperty.Register(
      nameof(ImportAllCommand), typeof(ICommand), typeof(ImportExportWidget));

    public static readonly DependencyProperty ExportAllToCsvCommandProperty = DependencyProperty.Register(
      nameof(ExportAllToCsvCommand), typeof(ICommand), typeof(ImportExportWidget));

    public static readonly DependencyProperty ExportAllToJsonCommandProperty = DependencyProperty.Register(
      nameof(ExportAllToJsonCommand), typeof(ICommand), typeof(ImportExportWidget));

    public ICommand ImportAllCommand
    {
      get => (ICommand) GetValue(ImportAllCommandProperty);
      set => SetValue(ImportAllCommandProperty, value);
    }

    public ICommand ExportAllToCsvCommand
    {
      get => (ICommand) GetValue(ExportAllToCsvCommandProperty);
      set => SetValue(ExportAllToCsvCommandProperty, value);
    }

    public ICommand ExportAllToJsonCommand
    {
      get => (ICommand) GetValue(ExportAllToJsonCommandProperty);
      set => SetValue(ExportAllToJsonCommandProperty, value);
    }
  }
}