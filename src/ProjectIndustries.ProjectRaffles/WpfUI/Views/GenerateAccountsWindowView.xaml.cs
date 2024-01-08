using System;
using System.Reactive.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.WpfUI.Services;
using ReactiveUI;
using Splat;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  public partial class GenerateAccountsWindowView
  {
    private readonly IFieldPresentationManager _fieldPresentationManager =
      Locator.Current.GetService<IFieldPresentationManager>();

    public GenerateAccountsWindowView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);

      this.WhenAnyValue(_ => _.ViewModel)
        .Where(vm => vm != null)
        .Select(_ => _.WhenAnyValue(vm => vm.SelectedGenerator))
        .Switch()
        .Subscribe(generator =>
        {
          var fields = generator?.ConfigurationFields ?? Array.Empty<Field>();
          _fieldPresentationManager.DisplayFields(ConfigurationFieldsSurface, fields, 1);
        });
    }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
      ViewModel.Reset();
    }
  }
}