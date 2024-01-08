using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  /// <summary>
  /// Interaction logic for CaptchasView.xaml
  /// </summary>
  public partial class CaptchasView
  {
    public CaptchasView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
    }
  }
}
