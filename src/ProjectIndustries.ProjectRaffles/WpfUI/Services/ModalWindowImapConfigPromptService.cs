using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Services.Emails;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Emails;
using ProjectIndustries.ProjectRaffles.WpfUI.Views;
using Splat;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services
{
  public class ModalWindowImapConfigPromptService : IImapConfigPromptService
  {
    public Task<ImapConfig> PromptAsync(Email email, CancellationToken ct = default)
    {
      var wnd = new ImapConfigEditorView
      {
        ViewModel = Locator.Current.GetService<ImapConfigEditorViewModel>()
      };

      ImapConfig config = null;
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

      return Task.FromResult(config);
    }
  }
}