using System;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  /// <summary>
  /// Interaction logic for ImapConfigEditorView.xaml
  /// </summary>
  public partial class SmtpConfigEditorView
  {
    public SmtpConfigEditorView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
    }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
      ViewModel.Reset();
    }
  }
}
