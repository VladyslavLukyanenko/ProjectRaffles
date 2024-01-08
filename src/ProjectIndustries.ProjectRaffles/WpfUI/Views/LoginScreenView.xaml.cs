using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ReactiveUI;
using Splat;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  /// <summary>
  /// Interaction logic for LoginScreenView.xaml
  /// </summary>
  public partial class LoginScreenView
  {
    public LoginScreenView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
      ViewModel = Locator.Current.GetService<LoginScreenViewModel>();
    }
  }
}