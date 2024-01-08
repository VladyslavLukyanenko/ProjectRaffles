using System;

using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Accounts;
using ProjectIndustries.ProjectRaffles.WpfUI.Services;
using ReactiveUI;

using Splat;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  /// <summary>
  /// Interaction logic for GenerateAccountsView.xaml
  /// </summary>
  public partial class HeaderGenerateAccountsView
  {
    public HeaderGenerateAccountsView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);

      this.WhenActivated(d =>
      {
        d(ViewModel.GenerateCommand.Subscribe(_ =>
        {
          var factory = Locator.Current.GetService<IWindowFactory>();
          var wnd = factory.CreateWindow<GenerateAccountsWindowView, GenerateAccountsWindowViewModel>();
          wnd.ShowDialog();
        }));
      });
    }
  }
}
