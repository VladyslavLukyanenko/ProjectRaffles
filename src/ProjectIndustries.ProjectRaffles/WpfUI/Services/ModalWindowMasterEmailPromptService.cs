using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ProjectIndustries.ProjectRaffles.WpfUI.Views;
using Splat;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services
{
  public class ModalWindowMasterEmailPromptService : IMasterEmailPromptService
  {
    public Task<Email> PromptAsync(CancellationToken ct = default)
    {
      var wnd = new MasterEmailPromptView()
      {
        ViewModel = Locator.Current.GetService<MasterEmailPromptViewModel>()
      };

      Email config = null;
      wnd.ViewModel.SaveCommand
        .Subscribe(cfg =>
        {
          config = cfg;
          if (cfg != null)
          {
            wnd.Close();
          }
        });

      wnd.ShowDialog();

      return Task.FromResult(config);
    }
  }
}