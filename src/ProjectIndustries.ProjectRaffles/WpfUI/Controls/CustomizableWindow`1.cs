using System.Windows;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Controls
{
  public abstract class CustomizableWindow<TViewModel>
    : CustomizableWindow, IViewFor<TViewModel>, IViewFor, IActivatableView
    where TViewModel : ViewModelBase
  {
    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
      typeof(TViewModel), typeof(ReactiveWindow<TViewModel>), new PropertyMetadata((PropertyChangedCallback) null));

    /// <summary>Gets the binding root view model.</summary>
    public TViewModel BindingRoot => this.ViewModel;

    /// <inheritdoc />
    public TViewModel ViewModel
    {
      get => (TViewModel) this.GetValue(ViewModelProperty);
      set => this.SetValue(ViewModelProperty, (object) value);
    }

    object IViewFor.ViewModel
    {
      get => (object) this.ViewModel;
      set => this.ViewModel = (TViewModel) value;
    }
  }
}