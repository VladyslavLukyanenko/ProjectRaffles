using System;
using System.Threading.Tasks;
using System.Windows;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ProjectIndustries.ProjectRaffles.WpfUI.Views;
using Splat;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services
{
  public class ModalWindowValueResolverConfigurationPresenter : IValueResolverConfigurationPresenter
  {
    public Task ShowConfigurationWindowAsync(string title, params Field[] configurationFields)
    {
      var wnd = new ValueResolverConfigurationView
      {
        ViewModel = Locator.Current.GetService<ValueResolverConfigurationViewModel>(),
        Owner = Application.Current.MainWindow
      };

      wnd.ViewModel.Title = title;
      wnd.ViewModel.Fields = configurationFields;
      wnd.ViewModel.OkCommand
        .Subscribe(_ => wnd.Close());

      wnd.ShowDialog();

      return Task.CompletedTask;
    }
  }
}