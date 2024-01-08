using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  /// <summary>
  /// Interaction logic for AddCaptchaView.xaml
  /// </summary>
  public partial class AddCaptchaView
  {
    public AddCaptchaView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
    }
  }
}
