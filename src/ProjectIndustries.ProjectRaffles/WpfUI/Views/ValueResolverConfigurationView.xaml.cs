using System;
using System.Reactive.Disposables;
using ProjectIndustries.ProjectRaffles.WpfUI.Services;
using ReactiveUI;
using Splat;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  public partial class ValueResolverConfigurationView
  {
    public ValueResolverConfigurationView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
      this.WhenActivated(d =>
      {
        var fieldPresentationManager = Locator.Current.GetService<IFieldPresentationManager>();
        ViewModel.WhenAnyValue(_ => _.Fields)
          .Subscribe(fields =>
          {
            fieldPresentationManager.DisplayFields(FieldsContainer, fields);
          })
          .DisposeWith(d);
      });
    }
  }
}