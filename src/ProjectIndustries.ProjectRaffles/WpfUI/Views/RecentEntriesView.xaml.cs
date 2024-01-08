using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  public partial class RecentEntriesView
  {
    public RecentEntriesView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
    }
  }
}