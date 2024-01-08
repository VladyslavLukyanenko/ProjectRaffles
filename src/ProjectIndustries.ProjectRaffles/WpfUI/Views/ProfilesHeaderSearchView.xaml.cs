using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  public partial class ProfilesHeaderSearchView
  {
    public ProfilesHeaderSearchView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
    }
  }
}