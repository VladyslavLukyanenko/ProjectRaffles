using System;
using System.Reactive.Disposables;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  public partial class MasterEmailPromptView
  {
    public MasterEmailPromptView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
      this.WhenActivated(d =>
      {
        ViewModel.DismissCommand
          .Subscribe(_ => Close())
          .DisposeWith(d);
      });
    }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
      ViewModel.Reset();
    }
  }
}