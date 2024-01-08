using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using ProjectIndustries.ProjectRaffles.WpfUI.Services;
using ReactiveUI;
using Splat;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  public partial class TaskEditView
  {
    public TaskEditView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
      this.WhenActivated(d =>
      {
        var fieldPresentationManager = Locator.Current.GetService<IFieldPresentationManager>();
        IDisposable moduleChanged = null;
        ViewModel.WhenAnyValue(_ => _.SelectedModule)
          .Subscribe(module =>
          {
            moduleChanged?.Dispose();
            if (module == null)
            {
              AdditionalFieldsContainer.Children.Clear();
              return;
            }

            ShowModuleFields(module, fieldPresentationManager);
            moduleChanged = Observable.FromEventPattern(h => module.AdditionalFieldsChanged += h,
                h => module.AdditionalFieldsChanged -= h)
              .Subscribe(_ => ShowModuleFields(module, fieldPresentationManager));
          })
          .DisposeWith(d);
      });
    }
    private void ShowModuleFields(IRaffleModule module, IFieldPresentationManager presentationManager)
    {
      presentationManager.DisplayFields(AdditionalFieldsContainer, module?.AdditionalFields ?? Array.Empty<Field>());
    }
  }
}