using System;
using System.Reactive.Disposables;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  public partial class UpdateView
  {
    public UpdateView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
      this.WhenActivated(d =>
      {
        ViewModel.LaunchUpdaterCommand.Subscribe(_ =>
          {
            App.Current.Shutdown();
          })
          .DisposeWith(d);
      });
    }
  }
}