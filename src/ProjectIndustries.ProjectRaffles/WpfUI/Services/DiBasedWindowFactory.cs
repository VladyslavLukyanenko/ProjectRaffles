using System.Windows;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ReactiveUI;
using Splat;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services
{
  public class DiBasedWindowFactory : IWindowFactory
  {
    public Window CreateWindow<TWindow, TViewModel>(Window parent = null)
      where TViewModel : ViewModelBase
      where TWindow : Window, IViewFor<TViewModel>, new()
    {
      var window = new TWindow {ViewModel = Locator.Current.GetService<TViewModel>()};
      if (parent != null)
      {
        window.Owner = parent;
      }

      return window;
    }
  }
}