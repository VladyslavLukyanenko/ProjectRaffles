using System.Windows;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services
{
  public interface IWindowFactory
  {
    Window CreateWindow<TWindow, TViewModel>(Window parent = null)
      where TViewModel : ViewModelBase
      where TWindow : Window, IViewFor<TViewModel>, new();
  }
}