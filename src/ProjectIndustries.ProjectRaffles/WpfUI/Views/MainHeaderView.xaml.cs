using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  /// <summary>
  /// Interaction logic for MainHeaderView.xaml
  /// </summary>
  public partial class MainHeaderView
  {
    public MainHeaderView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
    }
  }
}
