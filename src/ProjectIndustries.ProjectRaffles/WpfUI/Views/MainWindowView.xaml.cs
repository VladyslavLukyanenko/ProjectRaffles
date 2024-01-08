using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ReactiveUI;
using Splat;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindowView
  {
    public MainWindowView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
      this.WhenActivated(d =>
      {
        Locator.Current.GetService<IToastNotificationManager>().Resume();
        this.OneWayBind(ViewModel, x => x.Router, x => x.RoutedViewHost.Router)
          .DisposeWith(d);
      });

      ViewModel = Locator.Current.GetService<MainWindowViewModel>();
      ViewModel.NavigationMenu.NavigationButtons.First().Command.Execute().Subscribe();

      KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
      var keyboard = e.KeyboardDevice;
      if (keyboard.IsKeyDown(Key.D)
          && (keyboard.Modifiers & ModifierKeys.Control) != 0
          && (keyboard.Modifiers & ModifierKeys.Shift) != 0)
      {
        ViewModel.MemoryDumpCreator.CreateMemoryDumpCommand.Execute().Subscribe();
      }
    }
  }
}