using System.Windows.Controls;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Proxies;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  public partial class ProxiesView
    : ReactiveUserControl<ProxiesViewModel>
  {
    public ProxiesView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
    }
  }
}