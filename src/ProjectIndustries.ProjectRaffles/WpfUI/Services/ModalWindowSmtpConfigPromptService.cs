using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Services.Emails;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Emails;
using ProjectIndustries.ProjectRaffles.WpfUI.Views;
using Splat;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services
{
  public class ModalWindowSmtpConfigPromptService : ISmtpConfigPromptService
  {
    public async Task<SmtpConfig> PromptAsync(Email email, CancellationToken ct = default)
    {
      SmtpConfig config = null;
      await Application.Current.Dispatcher.InvokeAsync(() =>
      {
        var wnd = new SmtpConfigEditorView
        {
          ViewModel = Locator.Current.GetService<SmtpConfigEditorViewModel>()
        };

        wnd.ViewModel.TargetEmail = email;
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
      });

      return config;
    }
  }
}